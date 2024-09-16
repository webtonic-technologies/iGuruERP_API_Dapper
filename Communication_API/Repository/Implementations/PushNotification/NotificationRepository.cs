using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;
using Communication_API.Repository.Interfaces.PushNotification;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.PushNotification
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbConnection _connection;

        public NotificationRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> TriggerNotification(TriggerNotificationRequest request)
        {
            // Step 1: Insert or update the push notification
            string sql;
            if (request.PushNotificationID == 0)
            {
                sql = @"INSERT INTO [tblPushNotificationMaster] 
                (PredefinedTemplateID, NotificationMessage, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime) 
                VALUES (@PredefinedTemplateID, @NotificationMessage, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime);
                SELECT CAST(SCOPE_IDENTITY() as int);";  // Get the newly inserted ID
            }
            else
            {
                sql = @"UPDATE [tblPushNotificationMaster] 
                SET PredefinedTemplateID = @PredefinedTemplateID, NotificationMessage = @NotificationMessage, UserTypeID = @UserTypeID, 
                    GroupID = @GroupID, Status = @Status, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime 
                WHERE PushNotificationID = @PushNotificationID";
            }

            // Execute the query and get the PushNotificationID
            var pushNotificationID = request.PushNotificationID == 0
                ? await _connection.ExecuteScalarAsync<int>(sql, request)
                : request.PushNotificationID;

            // Step 2: Handle the student or employee mappings
            if (pushNotificationID > 0)
            {
                // If updating, clear existing mappings
                if (request.PushNotificationID != 0)
                {
                    if (request.UserTypeID == 1)
                    {
                        string deleteStudentMappingSql = "DELETE FROM tblPushNotificationStudentMapping WHERE PushNotificationID = @PushNotificationID";
                        await _connection.ExecuteAsync(deleteStudentMappingSql, new { PushNotificationID = pushNotificationID });
                    }
                    else if (request.UserTypeID == 2)
                    {
                        string deleteEmployeeMappingSql = "DELETE FROM tblPushNotificationEmployeeMapping WHERE PushNotificationID = @PushNotificationID";
                        await _connection.ExecuteAsync(deleteEmployeeMappingSql, new { PushNotificationID = pushNotificationID });
                    }
                }

                // Insert into student or employee mapping tables based on UserTypeID
                if (request.UserTypeID == 1 && request.StudentIDs != null && request.StudentIDs.Count > 0)
                {
                    string insertStudentMappingSql = "INSERT INTO tblPushNotificationStudentMapping (PushNotificationID, StudentID) VALUES (@PushNotificationID, @StudentID)";
                    foreach (var studentID in request.StudentIDs)
                    {
                        await _connection.ExecuteAsync(insertStudentMappingSql, new { PushNotificationID = pushNotificationID, StudentID = studentID });
                    }
                }
                else if (request.UserTypeID == 2 && request.EmployeeIDs != null && request.EmployeeIDs.Count > 0)
                {
                    string insertEmployeeMappingSql = "INSERT INTO tblPushNotificationEmployeeMapping (PushNotificationID, EmployeeID) VALUES (@PushNotificationID, @EmployeeID)";
                    foreach (var employeeID in request.EmployeeIDs)
                    {
                        await _connection.ExecuteAsync(insertEmployeeMappingSql, new { PushNotificationID = pushNotificationID, EmployeeID = employeeID });
                    }
                }

                return new ServiceResponse<string>(true, "Operation Successful", "PushNotification added/updated successfully", StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating PushNotification", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<ServiceResponse<List<Notification>>> GetNotificationReport(GetNotificationReportRequest request)
        {
            // Determine the SQL query to use based on UserTypeID
            string sql = string.Empty;

            if (request.UserTypeID == 1) // Students
            {
                sql = @"
            SELECT 
                s.student_id AS StudentID,
                s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name AS StudentName,
                CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
                pnm.ScheduleDate AS [DateTime],
                pt.PredefinedTemplateMessage AS Message,
                pnm.Status
            FROM tblPushNotificationMaster pnm
            INNER JOIN tblPushNotificationStudentMapping psm ON pnm.PushNotificationID = psm.PushNotificationID
            INNER JOIN tbl_StudentMaster s ON psm.StudentID = s.student_id
            INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = pnm.GroupID
            INNER JOIN tbl_Class c ON gcsm.ClassID = c.class_id
            INNER JOIN tbl_Section sec ON gcsm.SectionID = sec.section_id
            INNER JOIN tblPredefinedTemplate pt ON pnm.PredefinedTemplateID = pt.PredefinedTemplateID
            WHERE pnm.ScheduleDate BETWEEN @StartDate AND @EndDate
            ORDER BY pnm.ScheduleDate
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
            }
            else if (request.UserTypeID == 2) // Employees
            {
                sql = @"
            SELECT 
                e.Employee_id AS EmployeeID,
                e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name AS EmployeeName,
                CONCAT(d.DepartmentName, '-', des.DesignationName) AS DepartmentDesignation,
                pnm.ScheduleDate AS [DateTime],
                pt.PredefinedTemplateMessage AS Message,
                pnm.Status
            FROM tblPushNotificationMaster pnm
            INNER JOIN tblPushNotificationEmployeeMapping pem ON pnm.PushNotificationID = pem.PushNotificationID
            INNER JOIN tbl_EmployeeMaster e ON pem.EmployeeID = e.Employee_id
            INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
            INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
            INNER JOIN tblPredefinedTemplate pt ON pnm.PredefinedTemplateID = pt.PredefinedTemplateID
            WHERE pnm.ScheduleDate BETWEEN @StartDate AND @EndDate
            ORDER BY pnm.ScheduleDate
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
            }

            // Calculate the offset for pagination
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Execute the query and map results to Notification class
            var result = await _connection.QueryAsync<Notification>(sql, new { request.StartDate, request.EndDate, Offset = offset, PageSize = request.PageSize });

            // Return the response based on UserTypeID
            if (result != null && result.Any())
            {
                return new ServiceResponse<List<Notification>>(true, "Notification Report Found", result.ToList(), 200);
            }
            else
            {
                return new ServiceResponse<List<Notification>>(false, "No records found", null, 404);
            }
        }

    }
}
