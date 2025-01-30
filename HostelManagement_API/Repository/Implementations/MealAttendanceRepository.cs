using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;

namespace HostelManagement_API.Repository.Implementations
{
    public class MealAttendanceRepository : IMealAttendanceRepository
    {
        private readonly string _connectionString;

        public MealAttendanceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ServiceResponse<string>> SetMealAttendance(SetMealAttendanceRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Parse AttendanceDate to DateTime
                DateTime attendanceDate = DateTime.ParseExact(request.AttendanceDate, "dd-MM-yyyy", null);

                // Delete previous meal attendance for the same student and date
                var deleteQuery = @"
                    DELETE FROM tblMealAttendance
                    WHERE StudentID = @StudentID
                    AND MealTypeID = @MealTypeID
                    AND AttendanceDate = @AttendanceDate
                    AND InstituteID = @InstituteID";

                await db.ExecuteAsync(deleteQuery, new
                {
                    request.StudentID,
                    request.MealTypeID,
                    AttendanceDate = attendanceDate,
                    request.InstituteID
                });

                // Insert new meal attendance record
                var insertQuery = @"
                    INSERT INTO tblMealAttendance 
                    (StudentID, MealTypeID, HostelID, AttendanceDate, StatusID, Remarks, InstituteID)
                    VALUES
                    (@StudentID, @MealTypeID, @HostelID, @AttendanceDate, @StatusID, @Remarks, @InstituteID)";

                await db.ExecuteAsync(insertQuery, new
                {
                    request.StudentID,
                    request.MealTypeID,
                    request.HostelID,
                    AttendanceDate = attendanceDate,
                    request.StatusID,
                    request.Remarks,
                    request.InstituteID
                });

                return new ServiceResponse<string>(true, "Meal Attendance Recorded Successfully", "Success", 200);
            }
        }


        public async Task<IEnumerable<GetMealAttendanceResponse>> GetMealAttendance(GetMealAttendanceRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Parse AttendanceDate to DateTime format
                DateTime attendanceDate = DateTime.ParseExact(request.AttendanceDate, "dd-MM-yyyy", null);

                // Start building the SQL query
                string sqlQuery = @"
                   SELECT 
                    sm.student_id AS StudentID,
                    CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
                    sm.Admission_Number AS AdmissionNumber,
                    CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection,
                    sa.StatusID,
                    sa.Remarks,
                    h.HostelName,  -- Added HostelName from tblHostel table
                    r.RoomName    -- Added RoomName from tblRoom table
                FROM tblHostelRoomAllocation hra
                LEFT OUTER JOIN tbl_StudentMaster sm ON hra.StudentID = sm.student_id
                LEFT OUTER JOIN tblMealAttendance sa ON sa.StudentID = sm.student_id 
                    AND sa.AttendanceDate = @AttendanceDate 
                    AND sa.MealTypeID = @MealTypeID
                LEFT OUTER JOIN tbl_Class c ON sm.class_id = c.class_id
                LEFT OUTER JOIN tbl_Section s ON sm.section_id = s.section_id 
                LEFT OUTER JOIN tblHostel h ON hra.HostelID = h.HostelID
                LEFT OUTER JOIN tblRoom r ON hra.RoomID = r.RoomID
                WHERE hra.HostelID = @HostelID
                AND hra.InstituteID = @InstituteID
                AND hra.IsAllocated = 1 
                AND hra.IsVacated = 0";

                // Add Search condition if provided
                if (!string.IsNullOrEmpty(request.Search))
                {
                    sqlQuery += @"
            AND (CONCAT(sm.First_Name, ' ', sm.Last_Name) LIKE @Search 
            OR sm.Admission_Number LIKE @Search)";
                }

                // Execute the query and return the results
                var result = await db.QueryAsync<GetMealAttendanceResponse>(sqlQuery, new
                {
                    AttendanceDate = attendanceDate,  // Use parsed DateTime
                    request.HostelID,
                    request.MealTypeID,
                    request.InstituteID,
                    Search = "%" + request.Search + "%"  // Add '%' for partial matching
                });

                return result.ToList();
            }
        }

    }
}
