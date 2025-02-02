using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Globalization;

namespace HostelManagement_API.Repository.Implementations
{
    public class VisitorLogRepository : IVisitorLogRepository
    {
        private readonly string _connectionString;

        public VisitorLogRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddUpdateVisitorLog(AddUpdateVisitorLogRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        // Convert DateOfVisit from string to DateTime
                        DateTime? dateOfVisit = string.IsNullOrEmpty(request.DateOfVisit) ? (DateTime?)null : DateTime.ParseExact(request.DateOfVisit, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        // Convert CheckInTime and CheckOutTime from string to DateTime
                        DateTime? checkInTime = string.IsNullOrEmpty(request.CheckInTime) ? (DateTime?)null : DateTime.ParseExact(request.CheckInTime, "hh:mm tt", CultureInfo.InvariantCulture);
                        DateTime? checkOutTime = string.IsNullOrEmpty(request.CheckOutTime) ? (DateTime?)null : DateTime.ParseExact(request.CheckOutTime, "hh:mm tt", CultureInfo.InvariantCulture);

                        string sqlQuery = request.HostelVisitorID == 0
                            ? @"INSERT INTO tblHostelVisitor (VisitorCode, VisitorName, NoOfVisitor, PhoneNo, StudentID, HostelID, RoomNo, RelationshipToStudent, DateOfVisit, Address, CheckInTime, CheckOutTime, PurposeOfVisit, UploadFile, InstituteID, IsActive) 
                        VALUES (@VisitorCode, @VisitorName, @NoOfVisitor, @PhoneNo, @StudentID, @HostelID, @RoomNo, @RelationshipToStudent, @DateOfVisit, @Address, @CheckInTime, @CheckOutTime, @PurposeOfVisit, @UploadFile, @InstituteID, @IsActive); 
                        SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblHostelVisitor 
                        SET VisitorCode = @VisitorCode, VisitorName = @VisitorName, NoOfVisitor = @NoOfVisitor, PhoneNo = @PhoneNo, StudentID = @StudentID, HostelID = @HostelID, RoomNo = @RoomNo, 
                            RelationshipToStudent = @RelationshipToStudent, DateOfVisit = @DateOfVisit, Address = @Address, CheckInTime = @CheckInTime, CheckOutTime = @CheckOutTime, 
                            PurposeOfVisit = @PurposeOfVisit, UploadFile = @UploadFile, InstituteID = @InstituteID, IsActive = @IsActive
                        WHERE HostelVisitorID = @HostelVisitorID";

                        var hostelVisitorId = request.HostelVisitorID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.VisitorCode, request.VisitorName, request.NoOfVisitor, request.PhoneNo, request.StudentID, request.HostelID, request.RoomNo, request.RelationshipToStudent, DateOfVisit = dateOfVisit, request.Address, CheckInTime = checkInTime, CheckOutTime = checkOutTime, request.PurposeOfVisit, request.UploadFile, request.InstituteID, request.IsActive }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new { request.VisitorCode, request.VisitorName, request.NoOfVisitor, request.PhoneNo, request.StudentID, request.HostelID, request.RoomNo, request.RelationshipToStudent, DateOfVisit = dateOfVisit, request.Address, CheckInTime = checkInTime, CheckOutTime = checkOutTime, request.PurposeOfVisit, request.UploadFile, request.InstituteID, request.IsActive, request.HostelVisitorID }, transaction);

                        if (request.HostelVisitorID == 0)
                        {
                            hostelVisitorId = hostelVisitorId;
                        }
                        else
                        {
                            hostelVisitorId = (int)request.HostelVisitorID;
                        }

                        transaction.Commit();
                        return hostelVisitorId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        public async Task<ServiceResponse<IEnumerable<VisitorLogResponse>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Count query with optional search and date range filter
                string countQuery = @"
            SELECT COUNT(*) 
            FROM tblHostelVisitor o
            INNER JOIN tbl_StudentMaster s ON o.StudentID = s.student_id
            WHERE o.InstituteID = @InstituteID AND o.IsActive = 1";

                // Add search filter to the count query if search parameter is provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    countQuery += " AND (o.VisitorCode LIKE @Search OR o.PhoneNo LIKE @Search)";
                }

                // Add date range filter to the count query if StartDate and EndDate are provided
                if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrEmpty(request.EndDate))
                {
                    countQuery += " AND o.DateOfVisit BETWEEN @StartDate AND @EndDate";
                }

                // Get total count with the search and date range filters
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new
                {
                    request.InstituteID,
                    Search = "%" + request.Search + "%",
                    StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture), // Convert StartDate to DateTime
                    EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)   // Convert EndDate to DateTime
                });

                // Main SQL query with optional search and date range filters
                string sqlQuery = @"
            SELECT 
                o.HostelVisitorID, 
                o.VisitorCode, 
                o.VisitorName, 
                o.NoOfVisitor, 
                o.PhoneNo, 
                o.StudentID, 
                o.HostelID, 
                o.RoomNo, 
                o.RelationshipToStudent, 
                o.DateOfVisit, 
                o.Address, 
                o.CheckInTime, 
                o.CheckOutTime, 
                o.PurposeOfVisit, 
                o.UploadFile, 
                o.InstituteID, 
                o.IsActive,
                -- New fields from other tables
                CONCAT(s.First_Name, ' ', s.Middle_Name, ' ', s.Last_Name) AS StudentName,
                CONCAT(c.class_name, ' ', sec.section_name) AS ClassSection
            FROM tblHostelVisitor o
            INNER JOIN tbl_StudentMaster s ON o.StudentID = s.student_id
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            WHERE o.InstituteID = @InstituteID AND o.IsActive = 1";

                // Add search filter to the main query if search parameter is provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    sqlQuery += " AND (o.VisitorCode LIKE @Search OR o.PhoneNo LIKE @Search)";
                }

                // Add date range filter to the main query if StartDate and EndDate are provided
                if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrEmpty(request.EndDate))
                {
                    sqlQuery += " AND o.DateOfVisit BETWEEN @StartDate AND @EndDate";
                }

                sqlQuery += " ORDER BY o.DateOfVisit OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                // Fetch visitor logs with the required data
                var visitorLogs = await db.QueryAsync<VisitorLogResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Search = "%" + request.Search + "%",
                    StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture), // Convert StartDate to DateTime
                    EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),   // Convert EndDate to DateTime
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                // Format DateOfVisit, CheckInTime, and CheckOutTime as required
                foreach (var log in visitorLogs)
                {
                    // Format DateOfVisit as "DD-MM-YYYY"
                    log.DateOfVisit = DateTime.Parse(log.DateOfVisit).ToString("dd-MM-yyyy");

                    // Format CheckInTime and CheckOutTime as "hh:mm tt"
                    log.CheckInTime = DateTime.Parse(log.CheckInTime).ToString("hh:mm tt");
                    log.CheckOutTime = DateTime.Parse(log.CheckOutTime).ToString("hh:mm tt");
                }

                return new ServiceResponse<IEnumerable<VisitorLogResponse>>(true, "Hostel Visitor retrieved successfully", visitorLogs, 200, totalCount);
            }
        }



        public async Task<VisitorLogResponse> GetVisitorLogById(int hostelVisitorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"
            SELECT 
                o.HostelVisitorID, 
                o.VisitorCode, 
                o.VisitorName, 
                o.NoOfVisitor, 
                o.PhoneNo, 
                o.StudentID, 
                o.HostelID, 
                o.RoomNo, 
                o.RelationshipToStudent, 
                o.DateOfVisit, 
                o.Address, 
                o.CheckInTime, 
                o.CheckOutTime, 
                o.PurposeOfVisit, 
                o.UploadFile, 
                o.InstituteID, 
                o.IsActive,
                -- New fields from other tables
                CONCAT(s.First_Name, ' ', s.Middle_Name, ' ', s.Last_Name) AS StudentName,
                CONCAT(c.class_name, ' ', sec.section_name) AS ClassSection
            FROM tblHostelVisitor o
            INNER JOIN tbl_StudentMaster s ON o.StudentID = s.student_id
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            WHERE o.HostelVisitorID = @HostelVisitorID";

                var visitorLog = await db.QueryFirstOrDefaultAsync<VisitorLogResponse>(sqlQuery, new { HostelVisitorID = hostelVisitorId });

                if (visitorLog != null)
                {
                    // Format DateOfVisit, CheckInTime, and CheckOutTime as required
                    visitorLog.DateOfVisit = DateTime.Parse(visitorLog.DateOfVisit).ToString("dd-MM-yyyy");
                    visitorLog.CheckInTime = DateTime.Parse(visitorLog.CheckInTime).ToString("hh:mm tt");
                    visitorLog.CheckOutTime = DateTime.Parse(visitorLog.CheckOutTime).ToString("hh:mm tt");
                }

                return visitorLog;
            }
        }


        public async Task<int> DeleteVisitorLog(int hostelVisitorId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblHostelVisitor SET IsActive = 0 WHERE HostelVisitorID = @HostelVisitorID";
                return await db.ExecuteAsync(sqlQuery, new { HostelVisitorID = hostelVisitorId });
            }
        }
    }
}
