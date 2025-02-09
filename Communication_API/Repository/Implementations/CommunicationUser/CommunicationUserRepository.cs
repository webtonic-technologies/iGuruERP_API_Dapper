using System.Data;
using Dapper;
using Communication_API.DTOs.Requests;
using Communication_API.DTOs.Responses;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Repository.Interfaces;

namespace Communication_API.Repository.Implementations
{
    public class CommunicationUserRepository : ICommunicationUserRepository
    {
        private readonly IDbConnection _connection;

        public CommunicationUserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<GetStudentListResponse>>> GetStudentList(GetStudentListRequest request)
        {
            string sql = @"
                SELECT
                    s.student_id AS StudentID,
                    LTRIM(RTRIM(
                        s.First_Name + ' ' +
                        ISNULL(s.Middle_Name + ' ', '') +
                        s.Last_Name
                    )) AS StudentName,
                    s.Roll_Number AS RollNumber,
                    s.Admission_Number AS AdmissionNumber,
                    c.class_name AS Class,
                    sec.section_name AS Section
                FROM tblGroupClassSectionMapping gcs
                JOIN tbl_StudentMaster s 
                    ON s.class_id = gcs.ClassID 
                   AND s.section_id = gcs.SectionID
                JOIN tbl_Class c 
                    ON s.class_id = c.class_id 
                   AND c.institute_id = @InstituteID
                JOIN tbl_Section sec
                    ON s.section_id = sec.section_id
                WHERE gcs.GroupID = @GroupID
                  AND (
                         (@Status = 3)
                      OR (@Status = 1 AND s.IsActive = 1)
                      OR (@Status = 2 AND s.IsActive = 0)
                  );";

            var result = await _connection.QueryAsync<GetStudentListResponse>(
                sql,
                new
                {
                    GroupID = request.GroupID,
                    Status = request.Status,
                    InstituteID = request.InstituteID
                }
            );

            var resultList = result.ToList();

            if (resultList.Any())
            {
                return new ServiceResponse<List<GetStudentListResponse>>(
                    true, "Student List Found", resultList, 200, resultList.Count
                );
            }
            else
            {
                return new ServiceResponse<List<GetStudentListResponse>>(
                    false, "No records found", null, 404
                );
            }
        }

        public async Task<ServiceResponse<List<GetEmployeeListResponse>>> GetEmployeeList(GetEmployeeListRequest request)
        {
            string sql = @"
                SELECT 
                    e.Employee_id AS EmployeeID,
                    LTRIM(RTRIM(
                        e.First_Name + ' ' + ISNULL(e.Middle_Name + ' ', '') + e.Last_Name
                    )) AS EmployeeName,
                    e.Employee_code_id AS EmployeeCode,
                    d.DepartmentName AS Department,
                    des.DesignationName AS Designation
                FROM tblGroupEmployeeMapping gem
                JOIN tbl_EmployeeProfileMaster e 
                    ON e.Department_id = gem.DepartmentID 
                   AND e.Designation_id = gem.DesignationID
                JOIN tbl_Department d
                    ON d.Department_id = gem.DepartmentID
                JOIN tbl_Designation des
                    ON des.Designation_id = gem.DesignationID
                WHERE gem.GroupID = @GroupID
                  AND e.Institute_id = @InstituteID
                  AND (
                         (@Status = 3)                   -- Return both active and inactive employees
                      OR (@Status = 1 AND e.Status = 1)   -- Only active employees
                      OR (@Status = 2 AND e.Status = 0)   -- Only inactive employees
                  );";

            var result = await _connection.QueryAsync<GetEmployeeListResponse>(
                sql,
                new
                {
                    GroupID = request.GroupID,
                    Status = request.Status,
                    InstituteID = request.InstituteID
                }
            );

            var resultList = result.ToList();

            if (resultList.Any())
            {
                return new ServiceResponse<List<GetEmployeeListResponse>>(
                    true, "Employee List Found", resultList, 200, resultList.Count
                );
            }
            else
            {
                return new ServiceResponse<List<GetEmployeeListResponse>>(
                    false, "No records found", null, 404
                );
            }
        }
    }
}
