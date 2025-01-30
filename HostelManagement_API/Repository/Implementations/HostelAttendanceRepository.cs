using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class HostelAttendanceRepository : IHostelAttendanceRepository
    {
        private readonly string _connectionString;

        public HostelAttendanceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ServiceResponse<string>> SetHostelAttendance(SetHostelAttendanceRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Ensure that the AttendanceDate is correctly formatted and parsed to DateTime
                DateTime attendanceDate = DateTime.ParseExact(request.AttendanceDate, "dd-MM-yyyy", null);

                // Delete previous attendance for the same student, institute, and date
                var deleteQuery = @"
            DELETE FROM tblHostelAttendance
            WHERE StudentID = @StudentID
            AND AttendanceTypeID = @AttendanceTypeID
            AND AttendanceDate = @AttendanceDate
            AND InstituteID = @InstituteID";

                var deleteResult = await db.ExecuteAsync(deleteQuery, new
                {
                    request.StudentID,
                    request.AttendanceTypeID,
                    AttendanceDate = attendanceDate,
                    request.InstituteID
                });

                // Insert the new attendance record
                var insertQuery = @"
            INSERT INTO tblHostelAttendance 
            (StudentID, AttendanceTypeID, HostelID, FloorID, AttendanceDate, StatusID, Remarks, InstituteID)
            VALUES
            (@StudentID, @AttendanceTypeID, @HostelID, @FloorID, @AttendanceDate, @StatusID, @Remarks, @InstituteID)";

                var result = await db.ExecuteAsync(insertQuery, new
                {
                    request.StudentID,
                    request.AttendanceTypeID,
                    request.HostelID,
                    request.FloorID,
                    AttendanceDate = attendanceDate,
                    request.StatusID,
                    request.Remarks,
                    request.InstituteID
                });

                return new ServiceResponse<string>(
                    success: true,
                    message: "Hostel Attendance Recorded Successfully",
                    data: "Success",
                    statusCode: 200,
                    totalCount: null
                );
            }
        }

        public async Task<IEnumerable<GetHostelAttendanceResponse>> GetHostelAttendance(GetHostelAttendanceRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Convert the string AttendanceDate to DateTime using the correct format
                DateTime attendanceDate;
                if (!DateTime.TryParseExact(request.AttendanceDate, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out attendanceDate))
                {
                    throw new ArgumentException("Invalid AttendanceDate format. Please use 'DD-MM-YYYY'.");
                }

                // Updated SQL Query
                string sqlQuery = @"
                SELECT 
                    sm.student_id AS StudentID,
                    CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
                    sm.Admission_Number AS AdmissionNumber,
                    CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection,
                    sa.StatusID,
                    sa.Remarks,  
                    r.RoomName -- Added RoomName from tblRoom table
                FROM tblHostelRoomAllocation hra
                LEFT OUTER JOIN tbl_StudentMaster sm ON hra.StudentID = sm.student_id
                LEFT OUTER JOIN tblHostelAttendance sa ON sa.StudentID = sm.student_id 
                    AND sa.AttendanceDate = @AttendanceDate
                    AND sa.AttendanceTypeID = @AttendanceTypeID
                LEFT OUTER JOIN tbl_Class c ON sm.class_id = c.class_id
                LEFT OUTER JOIN tbl_Section s ON sm.section_id = s.section_id 
                LEFT OUTER JOIN tblRoom r ON hra.RoomID = r.RoomID
                WHERE hra.HostelID = @HostelID
                AND hra.InstituteID = @InstituteID
                AND hra.IsAllocated = 1 
                AND hra.IsVacated = 0";

                var result = await db.QueryAsync<GetHostelAttendanceResponse>(sqlQuery, new
                {
                    AttendanceDate = attendanceDate, // passing the parsed DateTime value
                    request.HostelID,
                    request.AttendanceTypeID,
                    request.InstituteID
                });

                return result.ToList();
            }
        }


        public async Task<IEnumerable<GetHostelAttendanceTypeResponse>> GetHostelAttendanceTypes()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tblHostelAttendanceType";
                var attendanceTypes = await db.QueryAsync<GetHostelAttendanceTypeResponse>(query);
                return attendanceTypes;
            }
        }

        public async Task<IEnumerable<GetHostelAttendanceStatusResponse>> GetHostelAttendanceStatuses()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tblHostelAttendanceStatus";
                var attendanceStatuses = await db.QueryAsync<GetHostelAttendanceStatusResponse>(query);
                return attendanceStatuses;
            }
        }
    }
}
