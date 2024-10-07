using Dapper;
using System.Data;
using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.Repository.Interfaces;
using EventGallery_API.DTOs.ServiceResponse;
using OfficeOpenXml;


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
        ? @"INSERT INTO tblHolidays (HolidayName, FromDate, ToDate, Description, InstituteID, AcademicYearCode, IsActive) 
   VALUES (@HolidayName, @FromDate, @ToDate, @Description, @InstituteID, @AcademicYearCode, 1);
   SELECT CAST(SCOPE_IDENTITY() AS INT);"
        : @"UPDATE tblHolidays 
   SET HolidayName = @HolidayName, FromDate = @FromDate, ToDate = @ToDate, Description = @Description, 
       InstituteID = @InstituteID, AcademicYearCode = @AcademicYearCode, IsActive = 1
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
                request.AcademicYearCode // Updated parameter name to match new column name
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

        public async Task<List<HolidayResponse>> GetAllHolidays(string academicYearCode, int instituteID, string search)
        {
            var query = @"
                        SELECT h.HolidayID, h.HolidayName, h.FromDate, h.ToDate, h.Description, 
                               c.class_name AS ClassName, s.section_name AS SectionName, hcs.ClassID, hcs.SectionID
                        FROM tblHolidays h
                        LEFT JOIN tblHolidayClassSectionMapping hcs ON h.HolidayID = hcs.HolidayID
                        LEFT JOIN tbl_class c ON hcs.ClassID = c.class_id
                        LEFT JOIN tbl_section s ON hcs.SectionID = s.section_id
                        WHERE h.AcademicYearCode = @AcademicYearCode 
                          AND h.InstituteID = @InstituteID
                          AND (@Search IS NULL OR h.HolidayName LIKE '%' + @Search + '%') AND h.IsActive = 1";

            // Fetch the raw data from the query
            var rawHolidays = await _dbConnection.QueryAsync<dynamic>(query, new
            {
                AcademicYearCode = academicYearCode,  // Use the updated parameter name
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
        WHERE h.HolidayID = @HolidayID AND h.IsActive = 1";

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

        public async Task<ServiceResponse<byte[]>> ExportAllHolidays()
        {
            var query = @"
                SELECT 
                    h.HolidayID,
                    h.HolidayName,
                    CONCAT(CONVERT(VARCHAR, h.FromDate, 103), ' to ', CONVERT(VARCHAR, h.ToDate, 103)) AS Date,
                    h.Description,
                    c.class_name,
                    s.section_name
                FROM tblHolidays h
                LEFT JOIN tblHolidayClassSectionMapping hcsm ON hcsm.HolidayID = h.HolidayID
                LEFT JOIN tbl_Class c ON c.class_id = hcsm.ClassID
                LEFT JOIN tbl_Section s ON s.section_id = hcsm.SectionID";

            var holidays = await _dbConnection.QueryAsync(query);

            // Generate Excel
            using var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Holidays");

                // Add header row
                worksheet.Cells[1, 1].Value = "Sr. No";
                worksheet.Cells[1, 2].Value = "Holiday Name";
                worksheet.Cells[1, 3].Value = "Date";
                worksheet.Cells[1, 4].Value = "Class & Section";
                worksheet.Cells[1, 5].Value = "Description";

                int row = 2;
                int srNo = 1;

                foreach (var holiday in holidays)
                {
                    worksheet.Cells[row, 1].Value = srNo++;
                    worksheet.Cells[row, 2].Value = holiday.HolidayName;
                    worksheet.Cells[row, 3].Value = holiday.Date;
                    worksheet.Cells[row, 4].Value = $"{holiday.class_name} - {holiday.section_name}";
                    worksheet.Cells[row, 5].Value = holiday.Description;
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                package.Save();
            }

            stream.Position = 0;
            return new ServiceResponse<byte[]>(true, "Exported all holidays successfully.", stream.ToArray(), 200);
        }
    }
}
