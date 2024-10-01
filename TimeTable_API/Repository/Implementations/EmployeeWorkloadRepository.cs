using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;

namespace TimeTable_API.Repository.Implementations
{
    public class EmployeeWorkloadRepository : IEmployeeWorkloadRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeWorkloadRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<EmployeeWorkloadResponse>> GetEmployeeWorkload(EmployeeWorkloadRequest request)
        {
            var response = new ServiceResponse<EmployeeWorkloadResponse>(false, "Failed to retrieve workload", new EmployeeWorkloadResponse(), 500);

            try
            {
                // Fetch Employee Details
                var employeeDetailsQuery = @"
                    SELECT 
                        emp.EmpPhoto,
                        emp.First_Name + ' ' + emp.Last_Name AS EmployeeName,
                        emp.Employee_code_id AS EmployeeCode,
                        emp.EmailID,
                        emp.mobile_number AS MobileNumber,
                        dept.DepartmentName,
                        desig.DesignationName
                    FROM 
                        tbl_EmployeeProfileMaster emp
                    LEFT JOIN 
                        tbl_Department dept ON emp.Department_id = dept.Department_id
                    LEFT JOIN 
                        tbl_Designation desig ON emp.Designation_id = desig.Designation_id
                    WHERE 
                        emp.Employee_id = @EmployeeID AND emp.Institute_id = @InstituteID;";

                var employeeDetails = await _connection.QueryFirstOrDefaultAsync<EmployeeDetailsResponse>(
                    employeeDetailsQuery,
                    new { request.EmployeeID, request.InstituteID }
                );

                // Fetch Workload and Session Details
                var workloadDetailsQuery = @"
                    SELECT 
                        cls.class_name AS Class,
                        sec.section_name AS Section,
                        sub.SubjectName AS Subject,
                        COUNT(*) AS AssignedSession
                    FROM 
                        tblTimeTableSessionSubjectEmployee tse
                    JOIN 
                        tblTimeTableSessionMapping tsm ON tse.TTSessionID = tsm.TTSessionID
                    JOIN 
                        tblTimeTableSessions ses ON tsm.SessionID = ses.SessionID
                    JOIN 
                        tblTimeTableClassSession tcs ON tsm.GroupID = tcs.GroupID 
                    JOIN 
                        tbl_Class cls ON tcs.ClassID = cls.class_id
                    JOIN 
                        tbl_Section sec ON tcs.SectionID = sec.section_id
                    JOIN 
                        tbl_Subjects sub ON tse.SubjectID = sub.SubjectID
                    WHERE 
                        tse.EmployeeID = @EmployeeID 
                    GROUP BY 
                        cls.class_name, sec.section_name, sub.SubjectName;";

                var workloadDetails = await _connection.QueryAsync<WorkloadDetailsResponse>(
                    workloadDetailsQuery,
                    new { request.EmployeeID }
                );

                // Calculate Total Sessions and Assigned Sessions
                var sessionCountQuery = @"
                    SELECT 
                        COUNT(tsm.TTSessionID) AS TotalSessions,
                        SUM(CASE WHEN tse.EmployeeID = @EmployeeID THEN 1 ELSE 0 END) AS AssignedSessions
                    FROM 
                        tblTimeTableSessionMapping tsm
                    JOIN 
                        tblTimeTableSessions ses ON tsm.SessionID = ses.SessionID
                    LEFT JOIN 
                        tblTimeTableSessionSubjectEmployee tse ON tsm.TTSessionID = tse.TTSessionID
                    JOIN 
                        tblTimeTableClassSession tcs ON tsm.GroupID = tcs.GroupID
                    WHERE 
                        tsm.GroupID IN (SELECT GroupID FROM tblTimeTableGroups WHERE InstituteID = @InstituteID);";

                var sessionCount = await _connection.QueryFirstOrDefaultAsync<SessionCountResponse>(
                    sessionCountQuery,
                    new { request.EmployeeID, request.InstituteID }
                );

                // Combine results into the response object
                var result = new EmployeeWorkloadResponse
                {
                    EmployeeDetails = employeeDetails,
                    WorkloadDetails = workloadDetails.AsList(),
                    SessionCount = sessionCount
                };

                response = new ServiceResponse<EmployeeWorkloadResponse>(
                    true,
                    "Workload retrieved successfully",
                    result,
                    200
                );
            }
            catch (Exception ex)
            {
                response = new ServiceResponse<EmployeeWorkloadResponse>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }

            return response;
        }
    }
}
