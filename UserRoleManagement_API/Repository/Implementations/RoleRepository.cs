using Dapper;
using System.Data;
using UserRoleManagement_API.DTOs.Response;
using UserRoleManagement_API.DTOs.ServiceResponse;
using UserRoleManagement_API.Models;
using UserRoleManagement_API.Repository.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserRoleManagement_API.DTOs.Requests;

namespace UserRoleManagement_API.Repository.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _connection;

        public RoleRepository(IDbConnection connection)
        {
            _connection = connection;
        }
         
        //public async Task<ServiceResponse<CreateNewRoleResponse>> CreateNewRole(CreateNewRoleRequest request)
        //{
        //    if (_connection.State == ConnectionState.Closed)
        //    {
        //        _connection.Open();
        //    }

        //    try
        //    {
        //        // Insert into tblUserRole
        //        string roleQuery = @"INSERT INTO [tblUserRole] (UserRoleName, ApplicationTypeID, InstituteID, IsActive) 
        //                     VALUES (@UserRoleName, @ApplicationTypeID, @InstituteID, 1);
        //                     SELECT CAST(SCOPE_IDENTITY() as int)";
        //        int roleID = await _connection.QuerySingleAsync<int>(roleQuery, request);

        //        // Insert into tblUserRoleSettingMapping
        //        foreach (var module in request.Modules)
        //        {
        //            foreach (var submodule in module.Submodules)
        //            {
        //                string mappingQuery = @"INSERT INTO [tblUserRoleSettingMapping] (RoleID, ModuleID, SubmoduleID) 
        //                                VALUES (@RoleID, @ModuleID, @SubModuleID)";
        //                await _connection.ExecuteAsync(mappingQuery, new { RoleID = roleID, ModuleID = module.ModuleID, SubModuleID = submodule.SubModuleID });
        //            }
        //        }

        //        // Prepare the response
        //        var response = new CreateNewRoleResponse
        //        {
        //            RoleID = roleID,
        //            UserRoleName = request.UserRoleName,
        //            ApplicationTypeID = request.ApplicationTypeID,
        //            InstituteID = request.InstituteID,
        //            Modules = new List<ModuleResponse>()
        //        };

        //        foreach (var module in request.Modules)
        //        {
        //            var moduleResponse = new ModuleResponse
        //            {
        //                ModuleID = module.ModuleID,
        //                Submodules = module.Submodules.Select(s => new SubmoduleResponse { SubModuleID = s.SubModuleID }).ToList()
        //            };
        //            response.Modules.Add(moduleResponse);
        //        }

        //        return new ServiceResponse<CreateNewRoleResponse>(true, "Role Created Successfully", response, 201);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<CreateNewRoleResponse>(false, ex.Message, null, 500);
        //    }
        //    finally
        //    {
        //        if (_connection.State == ConnectionState.Open)
        //        {
        //            _connection.Close();
        //        }
        //    }
        //}


        public async Task<ServiceResponse<CreateNewRoleResponse>> CreateNewRole(CreateNewRoleRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            try
            {
                // Insert into tblUserRole
                string roleQuery = @"INSERT INTO [tblUserRole] (UserRoleName, InstituteID, IsActive) 
                             VALUES (@UserRoleName, @InstituteID, 1);
                             SELECT CAST(SCOPE_IDENTITY() as int)";
                int roleID = await _connection.QuerySingleAsync<int>(roleQuery, request);

                // Insert into tblUserRoleSettingMapping with ApplicationTypeID
                foreach (var module in request.Modules)
                {
                    foreach (var submodule in module.Submodules)
                    {
                        string mappingQuery = @"INSERT INTO [tblUserRoleSettingMapping] (RoleID, ApplicationTypeID, ModuleID, SubmoduleID) 
                                        VALUES (@RoleID, @ApplicationTypeID, @ModuleID, @SubModuleID)";
                        await _connection.ExecuteAsync(mappingQuery, new
                        {
                            RoleID = roleID,
                            ApplicationTypeID = request.ApplicationTypeID,  // Insert ApplicationTypeID into tblUserRoleSettingMapping
                            ModuleID = module.ModuleID,
                            SubModuleID = submodule.SubModuleID
                        });
                    }
                }

                // Prepare the response
                var response = new CreateNewRoleResponse
                {
                    RoleID = roleID,
                    UserRoleName = request.UserRoleName,
                    ApplicationTypeID = request.ApplicationTypeID,
                    InstituteID = request.InstituteID,
                    Modules = new List<ModuleResponse>()
                };

                foreach (var module in request.Modules)
                {
                    var moduleResponse = new ModuleResponse
                    {
                        ModuleID = module.ModuleID,
                        Submodules = module.Submodules.Select(s => new SubmoduleResponse { SubModuleID = s.SubModuleID }).ToList()
                    };
                    response.Modules.Add(moduleResponse);
                }

                return new ServiceResponse<CreateNewRoleResponse>(true, "Role Created Successfully", response, 201);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CreateNewRoleResponse>(false, ex.Message, null, 500);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public async Task<ServiceResponse<List<GetUserRolesResponse>>> GetUserRoles(GetUserRolesRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            try
            {
                // Query to fetch roles and associated employees based on InstituteID
                string sql = @"
                    SELECT ur.RoleID, ur.UserRoleName as RoleName, urm.EmployeeID, 
                           CONCAT(epm.First_Name, ' ', epm.Last_Name) AS EmployeeName
                    FROM tblUserRole ur
                    LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
                    LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
                    WHERE ur.InstituteID = @InstituteID";

                var result = await _connection.QueryAsync<GetUserRolesResponse, EmployeeResponse, GetUserRolesResponse>(
                    sql,
                    (role, employee) =>
                    {
                        // Grouping the data to ensure we get all employees for a role
                        var userRole = new GetUserRolesResponse
                        {
                            RoleID = role.RoleID,
                            RoleName = role.RoleName,
                            Employees = new List<EmployeeResponse>()
                        };

                        // Add employee data
                        userRole.Employees.Add(employee);
                        return userRole;
                    },
                    param: new { InstituteID = request.InstituteID },
                    splitOn: "EmployeeID"
                );

                var groupedResults = result
                    .GroupBy(r => new { r.RoleID, r.RoleName })
                    .Select(g => new GetUserRolesResponse
                    {
                        RoleID = g.Key.RoleID,
                        RoleName = g.Key.RoleName,
                        Employees = g.SelectMany(r => r.Employees).ToList()
                    })
                    .ToList();

                return new ServiceResponse<List<GetUserRolesResponse>>(true, "User Roles Retrieved Successfully", groupedResults, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetUserRolesResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> AssignRole(AssignRoleRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            try
            {
                // Query to insert role assignments for multiple employees
                string query = @"INSERT INTO [tblUserRoleMapping] (RoleID, EmployeeID)
                         VALUES (@RoleID, @EmployeeID)";

                // Iterate over EmployeeIDs and insert for each employee
                foreach (var employeeID in request.EmployeeIDs)
                {
                    await _connection.ExecuteAsync(query, new { RoleID = request.RoleID, EmployeeID = employeeID });
                }

                return new ServiceResponse<string>(true, "Role Assigned Successfully", "Success", 201);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }


    }
}
