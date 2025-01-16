using Communication_API.DTOs.Requests;
using Communication_API.DTOs.Requests.Configuration;
using Communication_API.DTOs.Responses.Configuration;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Configuration;
using Communication_API.Repository.Interfaces.Configuration;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.Configuration
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDbConnection _connection;

        public GroupRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateGroup(AddUpdateGroupRequest request)
        {
            string sql;
            if (request.GroupID == 0)
            {
                sql = @"INSERT INTO [tblCommunicationGroup] (GroupName, AcademicYearCode, TypeID, InstituteID, IsActive) 
                VALUES (@GroupName, @AcademicYearCode, @TypeID, @InstituteID, 1)
                SELECT CAST(SCOPE_IDENTITY() as int);";
            }
            else
            {
                var GroupData = await _connection.QueryFirstOrDefaultAsync<dynamic>("Select * from tblCommunicationGroup where GroupID = @GroupID", new { GroupID = request.GroupID });

                if (GroupData == null)
                {
                    return new ServiceResponse<string>(false, "Operation Failed", "Record Not Found", 400);
                }

                sql = @"UPDATE [tblCommunicationGroup] 
                SET GroupName = @GroupName, AcademicYearCode = @AcademicYearCode, TypeID = @TypeID, InstituteID = @InstituteID, IsActive = 1
                WHERE GroupID = @GroupID";
            }

            var groupId = await _connection.ExecuteScalarAsync<int>(sql, request);
            if (groupId > 0 || request.GroupID != 0)
            {
                var groupID = request.GroupID == 0 ? groupId : request.GroupID;

                // Delete previous mappings for GroupID from tblGroupClassSectionMapping
                string deleteClassSectionMappingsSql = @"DELETE FROM [tblGroupClassSectionMapping] WHERE GroupID = @GroupID";
                await _connection.ExecuteAsync(deleteClassSectionMappingsSql, new { GroupID = groupID });

                // Delete previous student assignments from StudentCommGroup
                string deleteStudentCommGroupSql = @"DELETE FROM [StudentCommGroup] WHERE GroupID = @GroupID";
                await _connection.ExecuteAsync(deleteStudentCommGroupSql, new { GroupID = groupID });

                // Delete previous employee mappings from tblGroupEmployeeMapping
                string deleteEmployeeMappingsSql = @"DELETE FROM [tblGroupEmployeeMapping] WHERE GroupID = @GroupID";
                await _connection.ExecuteAsync(deleteEmployeeMappingsSql, new { GroupID = groupID });

                // Delete previous employee assignments from EmployeeCommGroup
                string deleteEmployeeCommGroupSql = @"DELETE FROM [EmployeeCommGroup] WHERE GroupID = @GroupID";
                await _connection.ExecuteAsync(deleteEmployeeCommGroupSql, new { GroupID = groupID });

                // Process student-related data only if TypeID = 1
                if (request.TypeID == 1 && request.ClassSectionMappings != null && request.StudentIDs != null)
                {
                    foreach (var mapping in request.ClassSectionMappings)
                    {
                        string classSectionSql = @"INSERT INTO [tblGroupClassSectionMapping] (GroupID, ClassID, SectionID) 
                                           VALUES (@GroupID, @ClassID, @SectionID)";
                        await _connection.ExecuteAsync(classSectionSql, new
                        {
                            GroupID = groupID,
                            ClassID = mapping.ClassID,
                            SectionID = mapping.SectionID
                        });
                    }

                    foreach (var studentId in request.StudentIDs)
                    {
                        string studentCommSql = @"INSERT INTO [StudentCommGroup] (GroupID, StudentID) 
                                          VALUES (@GroupID, @StudentID)";
                        await _connection.ExecuteAsync(studentCommSql, new
                        {
                            GroupID = groupID,
                            StudentID = studentId
                        });
                    }
                }

                // Process employee-related data only if TypeID = 2
                if (request.TypeID == 2 && request.DepartmentDesignationMappings != null && request.EmployeeIDs != null)
                {
                    foreach (var mapping in request.DepartmentDesignationMappings)
                    {
                        string employeeMappingSql = @"INSERT INTO [tblGroupEmployeeMapping] (GroupID, DepartmentID, DesignationID) 
                                              VALUES (@GroupID, @DepartmentID, @DesignationID)";
                        await _connection.ExecuteAsync(employeeMappingSql, new
                        {
                            GroupID = groupID,
                            DepartmentID = mapping.DepartmentID,
                            DesignationID = mapping.DesignationID
                        });
                    }

                    foreach (var employeeId in request.EmployeeIDs)
                    {
                        string employeeCommSql = @"INSERT INTO [EmployeeCommGroup] (GroupID, EmployeeID) 
                                           VALUES (@GroupID, @EmployeeID)";
                        await _connection.ExecuteAsync(employeeCommSql, new
                        {
                            GroupID = groupID,
                            EmployeeID = employeeId
                        });
                    }
                }

                return new ServiceResponse<string>(true, "Operation Successful", "Group Added/Updated Successfully", 200);
            }
            return new ServiceResponse<string>(false, "Operation Failed", null, 400);
        }

        public async Task<ServiceResponse<List<GetAllGroupResponse>>> GetAllGroup(GetAllGroupRequest request)
        {
            // Base SQL query to count the total number of groups for pagination or metadata
            var countSql = @"
                    SELECT COUNT(*) 
                    FROM [tblCommunicationGroup] 
                    WHERE AcademicYearCode = @AcademicYearCode 
                        AND InstituteID = @InstituteID 
                        AND IsActive = 1
                        AND (@Search IS NULL OR GroupName LIKE '%' + @Search + '%')";  // Add the condition for search

            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new
            {
                request.AcademicYearCode,
                request.InstituteID,
                Search = request.Search // Pass the search term (could be null or empty)
            });

            // Query to fetch the group information with Type and corresponding count (students/employees)
            var sql = @"
                    SELECT 
                        cg.GroupID,
                        cg.AcademicYearCode,
                        cg.GroupName,
                        gut.UserType,
                        ISNULL(
                            CASE 
                                WHEN gut.UserType = 'Student' THEN 
                                    (SELECT COUNT(*) FROM [StudentCommGroup] scg WHERE scg.GroupID = cg.GroupID)
                                WHEN gut.UserType = 'Employee' THEN 
                                    (SELECT COUNT(*) FROM [EmployeeCommGroup] ecg WHERE ecg.GroupID = cg.GroupID)
                                ELSE 0
                            END, 0) AS Count
                    FROM [tblCommunicationGroup] cg
                    INNER JOIN [tblGroupUserType] gut ON gut.UserTypeID = cg.TypeID
                    WHERE cg.AcademicYearCode = @AcademicYearCode 
                        AND cg.InstituteID = @InstituteID 
                        AND cg.IsActive = 1
                        AND (@Search IS NULL OR cg.GroupName LIKE '%' + @Search + '%')  -- Apply the search condition here
                    ORDER BY cg.GroupID
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                AcademicYearCode = request.AcademicYearCode,
                InstituteID = request.InstituteID,
                Search = request.Search, // Pass the search term (could be null or empty)
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var groups = await _connection.QueryAsync<GetAllGroupResponse>(sql, parameters);

            // Returning the results with a service response
            return new ServiceResponse<List<GetAllGroupResponse>>(true, "Records Found", groups.ToList(), 200, totalCount);
        }


        public async Task<ServiceResponse<Group>> GetGroup(int GroupID)
        {
            var sql = "SELECT * FROM [tblCommunicationGroup] WHERE GroupID = @GroupID";
            var group = await _connection.QueryFirstOrDefaultAsync<Group>(sql, new { GroupID });
            return new ServiceResponse<Group>(true, "Record Found", group, 200);
        }

        public async Task<ServiceResponse<string>> DeleteGroup(int GroupID)
        {
            string query = "UPDATE [tblCommunicationGroup] SET IsActive = 0 WHERE GroupID = @GroupID";
            var result = await _connection.ExecuteAsync(query, new { GroupID });

            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Group has been deactivated successfully.", "Success", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Group deactivation failed.", "Failure", 400);
            }
        }


        public async Task<ServiceResponse<List<GetGroupUserTypeResponse>>> GetGroupUserTypes()
        {
            string sql = @"SELECT UserTypeID, UserType FROM tblGroupUserType";
            var result = await _connection.QueryAsync<GetGroupUserTypeResponse>(sql);

            var groupUserTypes = result.ToList();

            if (groupUserTypes.Count > 0)
            {
                return new ServiceResponse<List<GetGroupUserTypeResponse>>(true, "Records Found", groupUserTypes, 200, groupUserTypes.Count);
            }
            else
            {
                return new ServiceResponse<List<GetGroupUserTypeResponse>>(false, "No Records Found", new List<GetGroupUserTypeResponse>(), 404);
            }
        }


        public async Task<ServiceResponse<GetGroupMembersResponse>> GetGroupMembers(GetGroupMembersRequest request)
        {
            // Determine if the group is for Students or Employees
            string groupTypeQuery = @"SELECT gut.UserType 
                              FROM tblCommunicationGroup cg 
                              INNER JOIN tblGroupUserType gut ON cg.TypeID = gut.UserTypeID 
                              WHERE cg.GroupID = @GroupID AND cg.InstituteID = @InstituteID";

            var groupType = await _connection.ExecuteScalarAsync<string>(groupTypeQuery, new { GroupID = request.GroupID, InstituteID = request.InstituteID });

            GetGroupMembersResponse response = new GetGroupMembersResponse();
            response.GroupID = request.GroupID;

            // Fetch the group name
            string groupNameQuery = @"SELECT GroupName FROM tblCommunicationGroup WHERE GroupID = @GroupID";
            response.GroupName = await _connection.ExecuteScalarAsync<string>(groupNameQuery, new { GroupID = request.GroupID });

            // Prepare to collect the members
            response.Members = new List<MemberDetails>();

            if (groupType == "Student")
            {  
                string studentQuery = @"
                SELECT 
                    s.First_Name + ' ' + ISNULL(s.Middle_Name, '') + ' ' + s.Last_Name AS Name,
                    STRING_AGG(CONCAT(c.class_name, '-', sec.section_name), ', ') AS ClassSection,
                    spi.Mobile_Number AS MobileNumber
                FROM StudentCommGroup scg
                INNER JOIN tbl_StudentMaster s ON scg.StudentID = s.student_id
                INNER JOIN tblGroupClassSectionMapping gcsm ON gcsm.GroupID = scg.GroupID
                INNER JOIN tbl_Class c ON gcsm.ClassID = c.class_id
                INNER JOIN tbl_Section sec ON gcsm.SectionID = sec.section_id
                INNER JOIN tbl_StudentParentsInfo spi ON spi.Student_ID = s.student_id AND spi.Parent_Type_id = 1
                WHERE scg.GroupID = @GroupID
                GROUP BY s.First_Name, s.Middle_Name, s.Last_Name, spi.Mobile_Number;
";

                var students = await _connection.QueryAsync<MemberDetails>(studentQuery, new { GroupID = request.GroupID });

                // Map the response for students
                response.Members = students.Select(s => new MemberDetails
                {
                    Name = s.Name,
                    ClassSection = s.ClassSection,
                    MobileNumber = s.MobileNumber
                }).ToList();

                response.TotalCount = students.Count();
            }
            else if (groupType == "Employee")
            {
                // Query to get employee members along with department-designation and mobile number
                string employeeQuery = @"
                SELECT 
                    e.First_Name + ' ' + ISNULL(e.Middle_Name, '') + ' ' + e.Last_Name AS Name,
                    CONCAT(d.DepartmentName, '-', des.DesignationName) AS DepartmentDesignation,
                    e.Mobile_number as MobileNumber
                FROM EmployeeCommGroup ecg
                INNER JOIN tbl_EmployeeProfileMaster e ON ecg.EmployeeID = e.Employee_id
                INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
                INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
                WHERE ecg.GroupID = @GroupID";

                var employees = await _connection.QueryAsync<MemberDetails>(employeeQuery, new { GroupID = request.GroupID });

                // Map the response for employees
                response.Members = employees.Select(e => new MemberDetails
                {
                    Name = e.Name,
                    DepartmentDesignation = e.DepartmentDesignation,
                    MobileNumber = e.MobileNumber
                }).ToList();

                response.TotalCount = employees.Count();
            }

            return new ServiceResponse<GetGroupMembersResponse>(true, "Group Members Found", response, 200);
        }


    }
}
