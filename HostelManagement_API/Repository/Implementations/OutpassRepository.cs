using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class OutpassRepository : IOutpassRepository
    {
        private readonly string _connectionString;

        public OutpassRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
         
        public async Task<int> AddUpdateOutpass(AddUpdateOutpassRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        // Convert OutpassDate from string to DateTime
                        DateTime outpassDate = DateTime.ParseExact(request.OutpassDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        // Convert time strings (hh.mm tt format) to DateTime
                        DateTime departureTime = DateTime.ParseExact(request.DepartureTime, "hh.mm tt", CultureInfo.InvariantCulture);
                        DateTime expectedArrivalTime = DateTime.ParseExact(request.ExpectedArrivalTime, "hh.mm tt", CultureInfo.InvariantCulture);
                        DateTime entryTime = DateTime.ParseExact(request.EntryTime, "hh.mm tt", CultureInfo.InvariantCulture);

                        string sqlQuery = request.OutpassID == 0
                            ? @"INSERT INTO tblHostelOutpass (OutpassCode, OutpassDate, HostelID, StudentID, RoomNo, DepartureTime, ExpectedArrivalTime, EntryTime, Reason, Remarks, UploadFile, InstituteID, IsActive) 
                        VALUES (@OutpassCode, @OutpassDate, @HostelID, @StudentID, @RoomNo, @DepartureTime, @ExpectedArrivalTime, @EntryTime, @Reason, @Remarks, @UploadFile, @InstituteID, @IsActive); 
                        SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblHostelOutpass 
                        SET OutpassCode = @OutpassCode, OutpassDate = @OutpassDate, HostelID = @HostelID, StudentID = @StudentID, 
                            RoomNo = @RoomNo, DepartureTime = @DepartureTime, ExpectedArrivalTime = @ExpectedArrivalTime, 
                            EntryTime = @EntryTime, Reason = @Reason, Remarks = @Remarks, UploadFile = @UploadFile, 
                            InstituteID = @InstituteID, IsActive = @IsActive
                        WHERE OutpassID = @OutpassID";

                        var outpassId = request.OutpassID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new
                            {
                                request.OutpassCode,
                                OutpassDate = outpassDate,
                                request.HostelID,
                                request.StudentID,
                                request.RoomNo,
                                DepartureTime = departureTime,
                                ExpectedArrivalTime = expectedArrivalTime,
                                EntryTime = entryTime,
                                request.Reason,
                                request.Remarks,
                                request.UploadFile,
                                request.InstituteID,
                                request.IsActive
                            }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new
                            {
                                request.OutpassCode,
                                OutpassDate = outpassDate,
                                request.HostelID,
                                request.StudentID,
                                request.RoomNo,
                                DepartureTime = departureTime,
                                ExpectedArrivalTime = expectedArrivalTime,
                                EntryTime = entryTime,
                                request.Reason,
                                request.Remarks,
                                request.UploadFile,
                                request.InstituteID,
                                request.IsActive,
                                request.OutpassID
                            }, transaction);

                        if (request.OutpassID == 0)
                        {
                            outpassId = outpassId;
                        }
                        else
                        {
                            outpassId = (int)request.OutpassID;
                        }

                        transaction.Commit();
                        return outpassId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<ServiceResponse<IEnumerable<OutpassResponse>>> GetAllOutpass(GetAllOutpassRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Convert StartDate and EndDate from string to DateTime if they are provided
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(request.StartDate))
                {
                    startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(request.EndDate))
                {
                    endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                // Count query with optional search and date range filters
                string countQuery = @"
            SELECT COUNT(*) 
            FROM tblHostelOutpass o
            INNER JOIN tbl_StudentMaster s ON o.StudentID = s.student_id
            WHERE o.InstituteID = @InstituteID AND o.IsActive = 1
        ";

                // Add search filter to the count query if search parameter is provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    countQuery += " AND (CONCAT(s.First_Name, ' ', s.Middle_Name, ' ', s.Last_Name) LIKE @Search OR o.OutpassCode LIKE @Search)";
                }

                // Add date range filter to the count query if StartDate and EndDate are provided
                if (startDate.HasValue && endDate.HasValue)
                {
                    countQuery += " AND o.OutpassDate BETWEEN @StartDate AND @EndDate";
                }

                // Get total count with the search and date range filters
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new
                {
                    request.InstituteID,
                    Search = "%" + request.Search + "%",
                    StartDate = startDate,
                    EndDate = endDate
                });

                // Main SQL query with optional search and date range filters
                string sqlQuery = @"
            SELECT 
                o.OutpassID, 
                o.OutpassCode, 
                o.OutpassDate, 
                o.HostelID, 
                o.StudentID, 
                o.RoomNo, 
                o.DepartureTime, 
                o.ExpectedArrivalTime, 
                o.EntryTime, 
                o.Reason, 
                o.Remarks, 
                o.UploadFile, 
                o.InstituteID, 
                o.IsActive, 
                -- New fields from other tables
                CONCAT(s.First_Name, ' ', s.Middle_Name, ' ', s.Last_Name) AS StudentName,
                CONCAT(c.class_name, ' ', sec.section_name) AS ClassSection,
                h.HostelName,
                r.RoomName
            FROM tblHostelOutpass o
            INNER JOIN tbl_StudentMaster s ON o.StudentID = s.student_id
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            INNER JOIN tblHostel h ON o.HostelID = h.HostelID
            INNER JOIN tblRoom r ON h.HostelID = r.HostelID 
            WHERE o.InstituteID = @InstituteID AND o.IsActive = 1
        ";

                // Add search filter to the main query if search parameter is provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    sqlQuery += " AND (CONCAT(s.First_Name, ' ', s.Middle_Name, ' ', s.Last_Name) LIKE @Search OR o.OutpassCode LIKE @Search)";
                }

                // Add date range filter to the main query if StartDate and EndDate are provided
                if (startDate.HasValue && endDate.HasValue)
                {
                    sqlQuery += " AND o.OutpassDate BETWEEN @StartDate AND @EndDate";
                }

                sqlQuery += " ORDER BY o.OutpassDate OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                // Fetch the data
                var outpasses = await db.QueryAsync<OutpassResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Search = "%" + request.Search + "%",
                    StartDate = startDate,
                    EndDate = endDate,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                // Format OutpassDate and time fields for each record
                foreach (var outpass in outpasses)
                {
                    // Format OutpassDate as "dd-MM-yyyy"
                    outpass.OutpassDate = DateTime.Parse(outpass.OutpassDate).ToString("dd-MM-yyyy");

                    // Format time fields as "hh:mm tt"
                    outpass.DepartureTime = DateTime.Parse(outpass.DepartureTime).ToString("hh:mm tt");
                    outpass.ExpectedArrivalTime = DateTime.Parse(outpass.ExpectedArrivalTime).ToString("hh:mm tt");
                    outpass.EntryTime = DateTime.Parse(outpass.EntryTime).ToString("hh:mm tt");
                }

                return new ServiceResponse<IEnumerable<OutpassResponse>>(true, "Outpass retrieved successfully", outpasses, 200, totalCount);
            }
        }


        public async Task<OutpassResponse> GetOutpassById(int outpassId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"
                SELECT 
                    o.OutpassID, 
                    o.OutpassCode, 
                    o.OutpassDate, 
                    o.HostelID, 
                    o.StudentID, 
                    o.RoomNo, 
                    o.DepartureTime, 
                    o.ExpectedArrivalTime, 
                    o.EntryTime, 
                    o.Reason, 
                    o.Remarks, 
                    o.UploadFile, 
                    o.InstituteID, 
                    o.IsActive, 
                    -- New fields from other tables
                    CONCAT(s.First_Name, ' ', s.Middle_Name, ' ', s.Last_Name) AS StudentName,
                    CONCAT(c.class_name, ' ', sec.section_name) AS ClassSection,
                    h.HostelName,
                    r.RoomName
                FROM tblHostelOutpass o
                INNER JOIN tbl_StudentMaster s ON o.StudentID = s.student_id
                INNER JOIN tbl_Class c ON s.class_id = c.class_id
                INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
                INNER JOIN tblHostel h ON o.HostelID = h.HostelID
                INNER JOIN tblRoom r ON h.HostelID = r.HostelID 
                WHERE o.OutpassID = @OutpassID";

                var outpass = await db.QueryFirstOrDefaultAsync<OutpassResponse>(sqlQuery, new { OutpassID = outpassId });

                if (outpass != null)
                {
                    // Format OutpassDate as "dd-MM-yyyy"
                    outpass.OutpassDate = DateTime.Parse(outpass.OutpassDate).ToString("dd-MM-yyyy");

                    // Format time fields as "hh:mm tt"
                    outpass.DepartureTime = DateTime.Parse(outpass.DepartureTime).ToString("hh:mm tt");
                    outpass.ExpectedArrivalTime = DateTime.Parse(outpass.ExpectedArrivalTime).ToString("hh:mm tt");
                    outpass.EntryTime = DateTime.Parse(outpass.EntryTime).ToString("hh:mm tt");
                }

                // Return the formatted outpass record or null if not found
                return outpass;
            }
        }
         
        public async Task<int> DeleteOutpass(int outpassId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblHostelOutpass SET IsActive = 0 WHERE OutpassID = @OutpassID";
                return await db.ExecuteAsync(sqlQuery, new { OutpassID = outpassId });
            }
        }
    }
}
