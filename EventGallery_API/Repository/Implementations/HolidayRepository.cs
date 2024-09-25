using Dapper;
using System.Data;
using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.Repository.Interfaces;

namespace EventGallery_API.Repository.Implementations
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly IDbConnection _dbConnection;

        public HolidayRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> AddUpdateHoliday(HolidayRequest request)
        {
            var query = request.HolidayID == 0
        ? @"INSERT INTO tblHolidays (HolidayName, FromDate, ToDate, Description, InstituteID, AcademicYearID, IsActive) 
           VALUES (@HolidayName, @FromDate, @ToDate, @Description, @InstituteID, @AcademicYearID, 1);
           SELECT CAST(SCOPE_IDENTITY() AS INT);"
        : @"UPDATE tblHolidays 
           SET HolidayName = @HolidayName, FromDate = @FromDate, ToDate = @ToDate, Description = @Description, 
               InstituteID = @InstituteID, AcademicYearID = @AcademicYearID, IsActive = 1
           WHERE HolidayID = @HolidayID; 
           SELECT @HolidayID;";

            // Execute the query and pass the FromDate and ToDate as already formatted in the ISO format
            var holidayID = await _dbConnection.QuerySingleAsync<int>(query, new
            {
                request.HolidayID,
                request.HolidayName,
                request.FromDate,  // Date is already in "yyyy-MM-dd" format
                request.ToDate,    // Date is already in "yyyy-MM-dd" format
                request.Description,
                request.InstituteID,
                request.AcademicYearID
            });


            // Delete old ClassSection mappings for this holiday
            await _dbConnection.ExecuteAsync("DELETE FROM tblHolidayClassSectionMapping WHERE HolidayID = @HolidayID", new { HolidayID = holidayID });

            // Insert new ClassSection mappings into tblHolidayClassSectionMapping
            if (request.ClassSection != null && request.ClassSection.Any())
            {
                var classSectionQuery = @"INSERT INTO tblHolidayClassSectionMapping (HolidayID, ClassID, SectionID)
                                  VALUES (@HolidayID, @ClassID, @SectionID)";

                foreach (var classSection in request.ClassSection)
                {
                    await _dbConnection.ExecuteAsync(classSectionQuery, new
                    {
                        HolidayID = holidayID,
                        classSection.ClassID,
                        classSection.SectionID
                    });
                }
            }

            // Return the HolidayID after insert/update
            return holidayID;
        }

        public async Task<List<HolidayResponse>> GetAllHolidays(int academicYearID, int instituteID, string search)
        {
            var query = @"
        SELECT h.HolidayID, h.HolidayName, h.FromDate, h.ToDate, h.Description, 
               c.class_name AS ClassName, s.section_name AS SectionName, hcs.ClassID, hcs.SectionID
        FROM tblHolidays h
        LEFT JOIN tblHolidayClassSectionMapping hcs ON h.HolidayID = hcs.HolidayID
        LEFT JOIN tbl_class c ON hcs.ClassID = c.class_id
        LEFT JOIN tbl_section s ON hcs.SectionID = s.section_id
        WHERE h.AcademicYearID = @AcademicYearID 
          AND h.InstituteID = @InstituteID
          AND (@Search IS NULL OR h.HolidayName LIKE '%' + @Search + '%')";

            // Fetch the raw data from the query
            var rawHolidays = await _dbConnection.QueryAsync<dynamic>(query, new
            {
                AcademicYearID = academicYearID,
                InstituteID = instituteID,
                Search = search
            });

            // Group the results by HolidayID to avoid duplicates and aggregate ClassSection data
            var holidays = rawHolidays.GroupBy(h => new
            {
                h.HolidayID,
                h.HolidayName,
                h.FromDate,
                h.ToDate,
                h.Description
            })
            .Select(g => new HolidayResponse
            {
                HolidayID = g.Key.HolidayID,
                HolidayName = g.Key.HolidayName,
                FromDate = g.Key.FromDate,
                ToDate = g.Key.ToDate,
                Description = g.Key.Description,
                Date = $"{g.Key.FromDate:dd-MM-yyyy} to {g.Key.ToDate:dd-MM-yyyy}", // Format the date
                ClassSection = g
                    .Where(x => x.ClassID != null && x.SectionID != null)
                    .Select(cs => new ClassSectionResponse
                    {
                        Class = cs.ClassName,
                        Section = cs.SectionName
                    })
                    .ToList()
            }).ToList();

            return holidays;
        }


        public async Task<HolidayResponse> GetHoliday(int holidayID)
        {
            var query = @"
        SELECT h.HolidayID, h.HolidayName, h.FromDate, h.ToDate, h.Description, 
               c.class_name AS ClassName, s.section_name AS SectionName, hcs.ClassID, hcs.SectionID
        FROM tblHolidays h
        LEFT JOIN tblHolidayClassSectionMapping hcs ON h.HolidayID = hcs.HolidayID
        LEFT JOIN tbl_class c ON hcs.ClassID = c.class_id
        LEFT JOIN tbl_section s ON hcs.SectionID = s.section_id
        WHERE h.HolidayID = @HolidayID";

            var rawHoliday = await _dbConnection.QueryAsync<dynamic>(query, new { HolidayID = holidayID });

            // Group the holiday and class-section mappings
            var holiday = rawHoliday.GroupBy(h => new
            {
                h.HolidayID,
                h.HolidayName,
                h.FromDate,
                h.ToDate,
                h.Description
            })
            .Select(g => new HolidayResponse
            {
                HolidayID = g.Key.HolidayID,
                HolidayName = g.Key.HolidayName,
                FromDate = g.Key.FromDate,
                ToDate = g.Key.ToDate,
                Description = g.Key.Description,
                Date = $"{g.Key.FromDate:dd-MM-yyyy} to {g.Key.ToDate:dd-MM-yyyy}",
                ClassSection = g
                    .Where(cs => cs.ClassID != null && cs.SectionID != null)
                    .Select(cs => new ClassSectionResponse
                    {
                        Class = cs.ClassName,
                        Section = cs.SectionName
                    })
                    .ToList()
            }).FirstOrDefault();

            return holiday;
        }


        public async Task<bool> DeleteHoliday(int holidayID)
        {
            var query = "UPDATE tblHolidays SET IsActive = 0 WHERE HolidayID = @HolidayID";
            var affectedRows = await _dbConnection.ExecuteAsync(query, new { HolidayID = holidayID });

            // Return true if any rows were affected, otherwise return false (holiday not found).
            return affectedRows > 0;
        }



        public async Task<List<HolidayResponse>> GetHolidaysByDateRange(DateRangeRequest request)
        {
            var query = @"SELECT * FROM tblHolidays WHERE InstituteID = @InstituteID AND HolidayDate BETWEEN @StartDate AND @EndDate";
            return (await _dbConnection.QueryAsync<HolidayResponse>(query, request)).ToList();
        }
    }
}
