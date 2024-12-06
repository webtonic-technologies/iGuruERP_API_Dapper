using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.Responses.PushNotification;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;
using Communication_API.Repository.Interfaces.PushNotification;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;

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
            // Step 1: Convert ScheduleDate and ScheduleTime to DateTime and TimeSpan
            DateTime parsedScheduleDate = DateTime.ParseExact(request.ScheduleDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            TimeSpan parsedScheduleTime = DateTime.ParseExact(request.ScheduleTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

            // Step 2: Insert or update the push notification
            string sql;
            if (request.PushNotificationID == 0)
            {
                sql = @"INSERT INTO [tblPushNotificationMaster] 
                (PredefinedTemplateID, NotificationMessage, UserTypeID, GroupID, ScheduleNow, ScheduleDate, ScheduleTime, AcademicYearCode, InstituteID) 
                VALUES (@PredefinedTemplateID, @NotificationMessage, @UserTypeID, @GroupID, @ScheduleNow, @ScheduleDate, @ScheduleTime, @AcademicYearCode, @InstituteID);
                SELECT CAST(SCOPE_IDENTITY() as int);";  // Get the newly inserted ID
            }
            else
            {
                sql = @"UPDATE [tblPushNotificationMaster] 
                SET PredefinedTemplateID = @PredefinedTemplateID, NotificationMessage = @NotificationMessage, UserTypeID = @UserTypeID, 
                    GroupID = @GroupID, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime,
                    AcademicYearCode = @AcademicYearCode, InstituteID = @InstituteID
                WHERE PushNotificationID = @PushNotificationID";
            }

            // Execute the query and get the PushNotificationID
            var pushNotificationID = request.PushNotificationID == 0
                ? await _connection.ExecuteScalarAsync<int>(sql, new
                {
                    request.PredefinedTemplateID,
                    request.NotificationMessage,
                    request.UserTypeID,
                    request.GroupID,
                    request.ScheduleNow,
                    ScheduleDate = parsedScheduleDate, // Pass parsed DateTime
                    ScheduleTime = parsedScheduleTime, // Pass parsed TimeSpan
                    request.AcademicYearCode,  // Pass AcademicYearCode
                    request.InstituteID,       // Pass InstituteID
                    request.PushNotificationID
                })
                : request.PushNotificationID;

            // Step 3: Handle the student or employee mappings
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

        //public async Task<ServiceResponse<List<PushNotificationStudentsResponse>>> GetPushNotificationStudent(PushNotificationStudentsRequest request)
        //{
        //    string userStatusCondition = request.UserTypeStatus switch
        //    {
        //        1 => "AND sm.isActive = 1",
        //        2 => "AND sm.isActive = 0",
        //        _ => "" // 3 means both Active and Inactive, so no filter
        //    };

        //    // Build the SQL query with IN to handle multiple GroupIDs
        //    string sql = @"
        //    SELECT 
        //        cg.GroupID,
        //        sm.student_id AS StudentID,
        //        sm.First_Name + ' ' + sm.Last_Name AS StudentName,
        //        sm.Roll_Number AS RollNumber,
        //        sm.Admission_Number AS AdmissionNumber,
        //        sm.class_id AS ClassID,
        //        c.class_name AS ClassName,
        //        sm.section_id AS SectionID,
        //        s.section_name AS SectionName,
        //        sm.isActive AS IsActive
        //    FROM tbl_StudentMaster sm
        //    INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.ClassID = sm.class_id AND gcsm.SectionID = sm.section_id
        //    INNER JOIN tbl_Section s ON s.section_id = sm.section_id
        //    INNER JOIN tbl_Class c ON c.class_id = sm.class_id
        //    INNER JOIN tblCommunicationGroup cg ON cg.GroupID IN @GroupIDs  -- Use IN for multiple GroupIDs
        //    WHERE sm.institute_id = @InstituteID
        //    " + userStatusCondition + @"
        //    ORDER BY sm.Roll_Number";

        //    // Execute the query with the provided InstituteID and GroupIDs
        //    var students = await _connection.QueryAsync<PushNotificationStudentsResponse>(sql, new { request.InstituteID, GroupIDs = request.GroupIDs });

        //    return new ServiceResponse<List<PushNotificationStudentsResponse>>(true, "Students fetched successfully", students.ToList(), 200);
        //}

        public async Task<ServiceResponse<List<PushNotificationStudentsResponse>>> GetPushNotificationStudent(PushNotificationStudentsRequest request)
        {
            // Define the user status condition based on the UserTypeStatus (Active, Inactive, or All)
            string userStatusCondition = request.UserTypeStatus switch
            {
                1 => "AND sm.isActive = 1",  // Active students
                2 => "AND sm.isActive = 0",  // Inactive students
                _ => ""  // UserTypeStatus = 3 means both Active and Inactive
            };

            // Build the SQL query with IN to handle multiple GroupIDs and fetch required student details
            string sql = @"
        SELECT DISTINCT
            scg.GroupID,
            scg.StudentID,
            sm.First_Name + ' ' + sm.Last_Name AS StudentName,
            sm.Roll_Number AS RollNumber,
            sm.Admission_Number AS AdmissionNumber,
            sm.class_id AS ClassID,
            c.class_name AS ClassName,
            sm.section_id AS SectionID,
            s.section_name AS SectionName,
            sm.isActive AS IsActive
        FROM StudentCommGroup scg
        INNER JOIN tbl_StudentMaster sm ON sm.student_id = scg.StudentID
        INNER JOIN tbl_Section s ON s.section_id = sm.section_id
        INNER JOIN tbl_Class c ON c.class_id = sm.class_id
        INNER JOIN tblCommunicationGroup cg ON cg.GroupID IN @GroupIDs  -- Use IN for multiple GroupIDs
        WHERE sm.institute_id = @InstituteID
        " + userStatusCondition + @"
        ORDER BY sm.Roll_Number";

            // Execute the query with the provided InstituteID and GroupIDs
            var students = await _connection.QueryAsync<PushNotificationStudentsResponse>(sql, new { request.InstituteID, GroupIDs = request.GroupIDs });

            // Return the results in a ServiceResponse
            return new ServiceResponse<List<PushNotificationStudentsResponse>>(true, "Students fetched successfully", students.ToList(), 200);
        }

        public async Task<ServiceResponse<List<PushNotificationEmployeesResponse>>> GetPushNotificationEmployee(PushNotificationEmployeesRequest request)
        {
            // Define the user status condition based on the UserTypeStatus (Active, Inactive, or All)
            string userStatusCondition = request.UserTypeStatus switch
            {
                1 => "AND epm.Status = 1",  // Active employees
                2 => "AND epm.Status = 0",  // Inactive employees
                _ => ""  // UserTypeStatus = 3 means both Active and Inactive
            };

            // Build the SQL query with IN to handle multiple GroupIDs and fetch required employee details
            string sql = @"
    SELECT 
        cg.GroupID,
        ecg.EmployeeID,
        epm.First_Name + ' ' + epm.Last_Name AS EmployeeName,
        epm.Employee_code_id AS EmployeeCode,
        epm.Department_id AS DepartmentID,
        d.DepartmentName AS DepartmentName,
        epm.Designation_id AS DesignationID,
        des.DesignationName AS DesignationName,
        epm.Status AS IsActive
    FROM EmployeeCommGroup ecg
    INNER JOIN tbl_EmployeeProfileMaster epm ON epm.Employee_id = ecg.EmployeeID
    INNER JOIN tbl_Department d ON d.Department_id = epm.Department_id
    INNER JOIN tbl_Designation des ON des.Designation_id = epm.Designation_id
    INNER JOIN tblCommunicationGroup cg ON cg.GroupID = ecg.GroupID  -- Ensure Employee is mapped to GroupID
    WHERE epm.Institute_id = @InstituteID
    AND cg.GroupID IN @GroupIDs  -- Filter by provided GroupIDs
    " + userStatusCondition + @"
    ORDER BY epm.Employee_code_id";

            // Execute the query with the provided InstituteID and GroupIDs
            var employees = await _connection.QueryAsync<PushNotificationEmployeesResponse>(sql, new { request.InstituteID, GroupIDs = request.GroupIDs });

            // Return the results in a ServiceResponse
            return new ServiceResponse<List<PushNotificationEmployeesResponse>>(true, "Employees fetched successfully", employees.ToList(), 200);
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

        public async Task InsertPushNotificationForStudent(int groupID, int instituteID, int studentID, string message, DateTime notificationDate, int statusID)
        {
            string sql = @"
                INSERT INTO tblPushNotificationStudent (GroupID, InstituteID, StudentID, PushNotificationMessage, PushNotificationDate, PushNotificationStatusID)
                VALUES (@GroupID, @InstituteID, @StudentID, @PushNotificationMessage, @PushNotificationDate, @PushNotificationStatusID)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                PushNotificationMessage = message,
                PushNotificationDate = notificationDate,
                PushNotificationStatusID = statusID
            });
        }

        public async Task InsertPushNotificationForEmployee(int groupID, int instituteID, int employeeID, string message, DateTime notificationDate, int statusID)
        {
            string sql = @"
                INSERT INTO tblPushNotificationEmployee (GroupID, InstituteID, EmployeeID, PushNotificationMessage, PushNotificationDate, PushNotificationStatusID)
                VALUES (@GroupID, @InstituteID, @EmployeeID, @PushNotificationMessage, @PushNotificationDate, @PushNotificationStatusID)";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                PushNotificationMessage = message,
                PushNotificationDate = notificationDate,
                PushNotificationStatusID = statusID
            });
        }

        public async Task UpdatePushNotificationStudentStatus(int groupID, int instituteID, int studentID, int pushNotificationStatusID)
        {
            string sql = @"
                UPDATE tblPushNotificationStudent
                SET PushNotificationStatusID = @PushNotificationStatusID
                WHERE GroupID = @GroupID
                  AND InstituteID = @InstituteID
                  AND StudentID = @StudentID";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                StudentID = studentID,
                PushNotificationStatusID = pushNotificationStatusID
            });
        }

        public async Task UpdatePushNotificationEmployeeStatus(int groupID, int instituteID, int employeeID, int pushNotificationStatusID)
        {
            string sql = @"
                UPDATE tblPushNotificationEmployee
                SET PushNotificationStatusID = @PushNotificationStatusID
                WHERE GroupID = @GroupID
                  AND InstituteID = @InstituteID
                  AND EmployeeID = @EmployeeID";

            await _connection.ExecuteAsync(sql, new
            {
                GroupID = groupID,
                InstituteID = instituteID,
                EmployeeID = employeeID,
                PushNotificationStatusID = pushNotificationStatusID
            });
        }

    }
}
