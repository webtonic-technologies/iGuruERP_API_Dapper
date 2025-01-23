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
using UserRoleManagement_API.DTOs.Responses;

namespace UserRoleManagement_API.Repository.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IDbConnection _connection;

        public RoleRepository(IDbConnection connection)
        {
            _connection = connection;
        }
         
        public async Task<ServiceResponse<CreateNewRoleResponse>> CreateNewRole(CreateNewRoleRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            try
            {
                // Check for duplicate UserRole in tblUserRole
                string checkUserRoleQuery = @"SELECT COUNT(*) FROM [tblUserRole] 
                                      WHERE UserRoleName = @UserRoleName AND InstituteID = @InstituteID";
                int existingRoleCount = await _connection.QuerySingleAsync<int>(checkUserRoleQuery, new
                {
                    UserRoleName = request.UserRoleName,
                    InstituteID = request.InstituteID
                });

                // If the role exists, return the existing RoleID, otherwise, insert a new role
                int roleID;
                if (existingRoleCount > 0)
                {
                    string getRoleIDQuery = @"SELECT RoleID FROM [tblUserRole] 
                                      WHERE UserRoleName = @UserRoleName AND InstituteID = @InstituteID";
                    roleID = await _connection.QuerySingleAsync<int>(getRoleIDQuery, new
                    {
                        UserRoleName = request.UserRoleName,
                        InstituteID = request.InstituteID
                    });

                    Console.WriteLine("Role already exists, using existing RoleID: " + roleID);
                }
                else
                {
                    string roleQuery = @"INSERT INTO [tblUserRole] (UserRoleName, InstituteID, IsActive, IsStudentRole) 
                                 VALUES (@UserRoleName, @InstituteID, 1, @IsStudentRole);
                                 SELECT CAST(SCOPE_IDENTITY() as int)";
                    roleID = await _connection.QuerySingleAsync<int>(roleQuery, request);
                    Console.WriteLine("New role created with RoleID: " + roleID);
                }

                // Insert into tblUserRoleSettingMapping with ApplicationTypeID
                foreach (var module in request.Modules)
                {
                    foreach (var submodule in module.Submodules)
                    {
                        // Insert functionalities if present
                        if (submodule.Functionalities != null && submodule.Functionalities.Any())
                        {
                            foreach (var functionality in submodule.Functionalities)
                            {
                                // Log the functionality ID to ensure it is not null or zero
                                Console.WriteLine($"Inserting FunctionalityID: {functionality.FunctionalityID}");

                                // Skip functionality with zero or invalid FunctionalityID
                                if (functionality.FunctionalityID == 0)
                                {
                                    Console.WriteLine($"Skipping FunctionalityID with value: {functionality.FunctionalityID}");
                                    continue;
                                }

                                // Check if the record already exists in tblUserRoleSettingMapping
                                string checkDuplicateQuery = @"SELECT COUNT(*) FROM [tblUserRoleSettingMapping] 
                                                        WHERE RoleID = @RoleID 
                                                        AND ApplicationTypeID = @ApplicationTypeID
                                                        AND ModuleID = @ModuleID
                                                        AND SubmoduleID = @SubModuleID
                                                        AND FunctionalityID = @FunctionalityID";

                                int count = await _connection.QuerySingleAsync<int>(checkDuplicateQuery, new
                                {
                                    RoleID = roleID,
                                    ApplicationTypeID = request.ApplicationTypeID,
                                    ModuleID = module.ModuleID,
                                    SubModuleID = submodule.SubModuleID,
                                    FunctionalityID = functionality.FunctionalityID
                                });

                                // If the record exists, delete it
                                if (count > 0)
                                {
                                    string deleteQuery = @"DELETE FROM [tblUserRoleSettingMapping]
                                                   WHERE RoleID = @RoleID 
                                                   AND ApplicationTypeID = @ApplicationTypeID
                                                   AND ModuleID = @ModuleID
                                                   AND SubmoduleID = @SubModuleID
                                                   AND FunctionalityID = @FunctionalityID";
                                    await _connection.ExecuteAsync(deleteQuery, new
                                    {
                                        RoleID = roleID,
                                        ApplicationTypeID = request.ApplicationTypeID,
                                        ModuleID = module.ModuleID,
                                        SubModuleID = submodule.SubModuleID,
                                        FunctionalityID = functionality.FunctionalityID
                                    });

                                    Console.WriteLine($"Deleted duplicate entry for FunctionalityID: {functionality.FunctionalityID}");
                                }

                                // Insert the new functionality record
                                string functionalityQuery = @"INSERT INTO [tblUserRoleSettingMapping] 
                                                      (RoleID, ApplicationTypeID, ModuleID, SubmoduleID, FunctionalityID) 
                                                      VALUES (@RoleID, @ApplicationTypeID, @ModuleID, @SubModuleID, @FunctionalityID)";

                                await _connection.ExecuteAsync(functionalityQuery, new
                                {
                                    RoleID = roleID,
                                    ApplicationTypeID = request.ApplicationTypeID,
                                    ModuleID = module.ModuleID,
                                    SubModuleID = submodule.SubModuleID,
                                    FunctionalityID = functionality.FunctionalityID
                                });

                                Console.WriteLine($"Inserted FunctionalityID: {functionality.FunctionalityID}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No functionalities to insert for SubmoduleID: " + submodule.SubModuleID);
                        }
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
                        Submodules = module.Submodules.Select(s => new SubmoduleResponse
                        {
                            SubModuleID = s.SubModuleID,
                            Functionalities = s.Functionalities?.Select(f => new FunctionalityResponse
                            {
                                FunctionalityID = f.FunctionalityID
                            }).ToList()
                        }).ToList()
                    };
                    response.Modules.Add(moduleResponse);
                }

                return new ServiceResponse<CreateNewRoleResponse>(true, "Role Created Successfully", response, 200);
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

        //Version 1.0
        //public async Task<ServiceResponse<List<GetUserRolesResponse>>> GetUserRoles(GetUserRolesRequest request)
        //{
        //    if (_connection.State == ConnectionState.Closed)
        //    {
        //        _connection.Open();
        //    }

        //    try
        //    {
        //        // SQL query to fetch roles and associated employees based on InstituteID and optional Search term
        //        string sql = @"
        //    SELECT ur.RoleID, ur.UserRoleName as RoleName, urm.EmployeeID, 
        //           CONCAT(epm.First_Name, ' ', epm.Last_Name) AS EmployeeName
        //    FROM tblUserRole ur
        //    LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
        //    LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
        //    WHERE ur.InstituteID = @InstituteID AND ur.IsActive = 1";

        //        // If a search term is provided, add a LIKE clause to the SQL query
        //        if (!string.IsNullOrEmpty(request.Search))
        //        {
        //            sql += " AND ur.UserRoleName LIKE @Search";
        //        }

        //        var result = await _connection.QueryAsync<GetUserRolesResponse, EmployeeResponse, GetUserRolesResponse>(
        //            sql,
        //            (role, employee) =>
        //            {
        //                // Grouping the data to ensure we get all employees for a role
        //                var userRole = new GetUserRolesResponse
        //                {
        //                    RoleID = role.RoleID,
        //                    RoleName = role.RoleName,
        //                    Employees = new List<EmployeeResponse>()
        //                };

        //                // Add employee data
        //                userRole.Employees.Add(employee);
        //                return userRole;
        //            },
        //            param: new { InstituteID = request.InstituteID, Search = "%" + request.Search + "%" }, // Perform LIKE search
        //            splitOn: "EmployeeID"
        //        );

        //        var groupedResults = result
        //            .GroupBy(r => new { r.RoleID, r.RoleName })
        //            .Select(g => new GetUserRolesResponse
        //            {
        //                RoleID = g.Key.RoleID,
        //                RoleName = g.Key.RoleName,
        //                Employees = g.SelectMany(r => r.Employees).ToList()
        //            })
        //            .ToList();

        //        return new ServiceResponse<List<GetUserRolesResponse>>(true, "User Roles Retrieved Successfully", groupedResults, 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<GetUserRolesResponse>>(false, ex.Message, null, 500);
        //    }
        //}


        //Version 2.0
        //public async Task<ServiceResponse<List<GetUserRolesResponse>>> GetUserRoles(GetUserRolesRequest request)
        //{
        //    if (_connection.State == ConnectionState.Closed)
        //    {
        //        _connection.Open();
        //    }

        //    try
        //    {
        //        // Updated SQL query to fetch roles and associated employees based on InstituteID and optional Search term
        //        string sql = @"
        //    SELECT ur.RoleID, ur.UserRoleName as RoleName, urm.EmployeeID, 
        //           CONCAT(epm.First_Name, ' ', epm.Last_Name) AS EmployeeName
        //    FROM tblUserRole ur
        //    LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
        //    LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
        //    WHERE ur.IsActive = 1 
        //    AND ur.IsDefault = 1
        //    AND ur.UserRoleName LIKE @Search

        //    UNION ALL

        //    SELECT ur.RoleID, ur.UserRoleName as RoleName, urm.EmployeeID, 
        //           CONCAT(epm.First_Name, ' ', epm.Last_Name) AS EmployeeName
        //    FROM tblUserRole ur
        //    LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
        //    LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
        //    WHERE ur.InstituteID = @InstituteID 
        //    AND ur.IsActive = 1 
        //    AND ur.IsDefault = 0
        //    AND ur.UserRoleName LIKE @Search";

        //        //// If a search term is provided, add a LIKE clause to the SQL query
        //        //if (!string.IsNullOrEmpty(request.Search))
        //        //{
        //        //    sql += " AND ur.UserRoleName LIKE @Search";
        //        //}

        //        var result = await _connection.QueryAsync<GetUserRolesResponse, EmployeeResponse, GetUserRolesResponse>(
        //            sql,
        //            (role, employee) =>
        //            {
        //                // Grouping the data to ensure we get all employees for a role
        //                var userRole = new GetUserRolesResponse
        //                {
        //                    RoleID = role.RoleID,
        //                    RoleName = role.RoleName,
        //                    Employees = new List<EmployeeResponse>()
        //                };

        //                // Add employee data
        //                userRole.Employees.Add(employee);
        //                return userRole;
        //            },
        //            param: new { InstituteID = request.InstituteID, Search = "%" + request.Search + "%" }, // Perform LIKE search
        //            splitOn: "EmployeeID"
        //        );

        //        // Group the results by RoleID and RoleName to combine employees for each role
        //        var groupedResults = result
        //            .GroupBy(r => new { r.RoleID, r.RoleName })
        //            .Select(g => new GetUserRolesResponse
        //            {
        //                RoleID = g.Key.RoleID,
        //                RoleName = g.Key.RoleName,
        //                Employees = g.SelectMany(r => r.Employees).ToList()
        //            })
        //            .ToList();

        //        return new ServiceResponse<List<GetUserRolesResponse>>(true, "User Roles Retrieved Successfully", groupedResults, 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<GetUserRolesResponse>>(false, ex.Message, null, 500);
        //    }
        //}

        //Version 3.0

        //public async Task<ServiceResponse<List<GetUserRolesResponse>>> GetUserRoles(GetUserRolesRequest request)
        //{
        //    if (_connection.State == ConnectionState.Closed)
        //    {
        //        _connection.Open();
        //    }

        //    try
        //    {


        //        // Modified count query to sum the counts from both queries
        //        string countSql = @"
        //SELECT SUM(totalCount) 
        //FROM (
        //    SELECT COUNT(DISTINCT ur.RoleID) AS totalCount
        //    FROM tblUserRole ur
        //    LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
        //    LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
        //    WHERE ur.IsActive = 1 
        //    AND ur.IsDefault = 1
        //    AND ur.UserRoleName LIKE @Search

        //    UNION ALL

        //    SELECT COUNT(DISTINCT ur.RoleID) AS totalCount
        //    FROM tblUserRole ur
        //    LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
        //    LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
        //    WHERE ur.InstituteID = @InstituteID 
        //    AND ur.IsActive = 1 
        //    AND ur.IsDefault = 0
        //    AND ur.UserRoleName LIKE @Search
        //) AS combinedCounts";

        //        var totalCount = await _connection.QuerySingleAsync<int>(countSql, new
        //        {
        //            InstituteID = request.InstituteID,
        //            Search = "%" + request.Search + "%"
        //        });


        //        // Paginated query to fetch roles and associated employees
        //        string sql = @"
        //        SELECT ur.RoleID, ur.UserRoleName as RoleName, urm.EmployeeID, 
        //               CONCAT(epm.First_Name, ' ', epm.Last_Name) AS EmployeeName
        //        FROM tblUserRole ur
        //        LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
        //        LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
        //        WHERE ur.IsActive = 1 
        //        AND ur.IsDefault = 1
        //        AND ur.UserRoleName LIKE @Search

        //        UNION ALL

        //        SELECT ur.RoleID, ur.UserRoleName as RoleName, urm.EmployeeID, 
        //               CONCAT(epm.First_Name, ' ', epm.Last_Name) AS EmployeeName
        //        FROM tblUserRole ur
        //        LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
        //        LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
        //        WHERE ur.InstituteID = @InstituteID 
        //        AND ur.IsActive = 1 
        //        AND ur.IsDefault = 0
        //        AND ur.UserRoleName LIKE @Search

        //        -- Apply pagination using OFFSET and FETCH
        //        ORDER BY ur.RoleID
        //        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        //        // Calculate the offset based on the page number and page size
        //        int offset = (request.PageNumber - 1) * request.PageSize;

        //        var result = await _connection.QueryAsync<GetUserRolesResponse, EmployeeResponse, GetUserRolesResponse>(
        //            sql,
        //            (role, employee) =>
        //            {
        //                // Ensure that if the role already exists, we just add the employee to that role
        //                var userRole = new GetUserRolesResponse
        //                {
        //                    RoleID = role.RoleID,
        //                    RoleName = role.RoleName,
        //                    Employees = new List<EmployeeResponse>()
        //                };

        //                // Add employee data
        //                userRole.Employees.Add(employee);
        //                return userRole;
        //            },
        //            param: new
        //            {
        //                InstituteID = request.InstituteID,
        //                Search = "%" + request.Search + "%",
        //                Offset = offset,
        //                PageSize = request.PageSize
        //            }, // Apply pagination parameters
        //            splitOn: "EmployeeID"
        //        );

        //        // Group the results by RoleID and RoleName to combine employees for each role
        //        var groupedResults = result
        //            .GroupBy(r => new { r.RoleID, r.RoleName })
        //            .Select(g => new GetUserRolesResponse
        //            {
        //                RoleID = g.Key.RoleID,
        //                RoleName = g.Key.RoleName,
        //                Employees = g.SelectMany(r => r.Employees).ToList()
        //            })
        //            .ToList();

        //        // Return the result with total count for pagination
        //        return new ServiceResponse<List<GetUserRolesResponse>>(true, "User Roles Retrieved Successfully", groupedResults, 200)
        //        {
        //            TotalCount = totalCount
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<List<GetUserRolesResponse>>(false, ex.Message, null, 500);
        //    }
        //}


        public async Task<ServiceResponse<List<GetUserRolesResponse>>> GetUserRoles(GetUserRolesRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            try
            {
                // Modified count query to sum the counts from both queries
                string countSql = @"
                SELECT SUM(totalCount) 
                FROM (
                    SELECT COUNT(DISTINCT ur.RoleID) AS totalCount
                    FROM tblUserRole ur
                    LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
                    LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
                    WHERE ur.IsActive = 1 
                    AND ur.IsDefault = 1
                    AND ur.UserRoleName LIKE @Search

                    UNION ALL

                    SELECT COUNT(DISTINCT ur.RoleID) AS totalCount
                    FROM tblUserRole ur
                    LEFT JOIN tblUserRoleMapping urm ON ur.RoleID = urm.RoleID
                    LEFT JOIN tbl_EmployeeProfileMaster epm ON urm.EmployeeID = epm.Employee_id
                    WHERE ur.InstituteID = @InstituteID 
                    AND ur.IsActive = 1 
                    AND ur.IsDefault = 0
                    AND ur.UserRoleName LIKE @Search
                ) AS combinedCounts";

                var totalCount = await _connection.QuerySingleAsync<int>(countSql, new
                {
                    InstituteID = request.InstituteID,
                    Search = "%" + request.Search + "%"
                });

                // Paginated query to fetch roles based on the search and InstituteID
                string sql = @"
            SELECT ur.RoleID, ur.UserRoleName as RoleName
            FROM tblUserRole ur
            WHERE ur.IsActive = 1 
            AND ur.UserRoleName LIKE @Search

            UNION ALL

            SELECT ur.RoleID, ur.UserRoleName as RoleName
            FROM tblUserRole ur
            WHERE ur.InstituteID = @InstituteID 
            AND ur.IsActive = 1 
            AND ur.IsDefault = 0
            AND ur.UserRoleName LIKE @Search

            -- Apply pagination using OFFSET and FETCH for roles
            ORDER BY ur.RoleID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                // Calculate the offset based on the page number and page size
                int offset = (request.PageNumber - 1) * request.PageSize;

                var roles = await _connection.QueryAsync<GetUserRolesResponse>(
                    sql,
                    new
                    {
                        InstituteID = request.InstituteID,
                        Search = "%" + request.Search + "%",
                        Offset = offset,
                        PageSize = request.PageSize
                    }
                );

                // For each role, get the associated employees
                foreach (var role in roles)
                {
                    string employeeSql = @"
                SELECT epm.Employee_id AS EmployeeID, CONCAT(epm.First_Name, ' ', epm.Last_Name) AS EmployeeName
                FROM tbl_EmployeeProfileMaster epm
                LEFT JOIN tblUserRoleMapping urm ON epm.Employee_id = urm.EmployeeID
                WHERE urm.RoleID = @RoleID";

                    var employees = await _connection.QueryAsync<EmployeeResponse>(employeeSql, new { RoleID = role.RoleID });
                    role.Employees = employees.ToList();
                }

                // Return the result with total count for pagination
                return new ServiceResponse<List<GetUserRolesResponse>>(true, "User Roles Retrieved Successfully", roles.ToList(), 200)
                {
                    TotalCount = totalCount
                };
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
                // Query to delete existing role assignments if they already exist
                string deleteQuery = @"DELETE FROM [tblUserRoleMapping] 
                               WHERE RoleID = @RoleID AND EmployeeID = @EmployeeID";

                // Iterate over EmployeeIDs to delete existing role assignments
                foreach (var employeeID in request.EmployeeIDs)
                {
                    await _connection.ExecuteAsync(deleteQuery, new { RoleID = request.RoleID, EmployeeID = employeeID });
                }

                // Query to insert role assignments for multiple employees
                string insertQuery = @"INSERT INTO [tblUserRoleMapping] (RoleID, EmployeeID)
                               VALUES (@RoleID, @EmployeeID)";

                // Insert new role assignments for each employee
                foreach (var employeeID in request.EmployeeIDs)
                {
                    await _connection.ExecuteAsync(insertQuery, new { RoleID = request.RoleID, EmployeeID = employeeID });
                }

                return new ServiceResponse<string>(true, "Role Assigned Successfully", "Success", 200);
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

        public async Task<bool> DeleteRoleAsync(DeleteRoleRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            try
            {
                // Update IsActive to false for the given RoleID and InstituteID
                string query = @"
                    UPDATE tblUserRole
                    SET IsActive = 0
                    WHERE RoleID = @RoleID AND InstituteID = @InstituteID";

                int rowsAffected = await _connection.ExecuteAsync(query, request);

                return rowsAffected > 0; // Return true if the role was updated successfully
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public async Task<GetRolesPermissionsResponse> GetRolesPermissionsAsync(GetRolesPermissionsRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            try
            {
                // SQL Query to get the required role permissions data
                string sql = @"
            SELECT ur.RoleID, ur.UserRoleName, at.ApplicationTypeID, at.ApplicationType, 
                   ur.InstituteID, ur.IsStudentRole, 
                   m.ModuleID, m.ModuleName, 
                   sm.SubModuleID, sm.SubModuleName, 
                   f.FunctionalityID, f.Functionality
            FROM tblUserRole ur
            LEFT JOIN tblUserRoleSettingMapping urm ON ur.RoleID = urm.RoleID
            LEFT JOIN tblModules m ON urm.ModuleID = m.ModuleID
            LEFT JOIN tblSubModule sm ON urm.SubmoduleID = sm.SubModuleID
            LEFT JOIN tblFunctionality f ON urm.FunctionalityID = f.FunctionalityID
            LEFT JOIN tblApplicationType at ON urm.ApplicationTypeID = at.ApplicationTypeID
            WHERE ur.RoleID = @RoleID AND ur.InstituteID = @InstituteID";

                var result = await _connection.QueryAsync<GetRolesPermissionsResponse, ModuleResponse1, SubmoduleResponse1, FunctionalityResponse1, GetRolesPermissionsResponse>(
                    sql,
                    (role, module, submodule, functionality) =>
                    {
                        // Build the response
                        var userRole = new GetRolesPermissionsResponse
                        {
                            RoleID = role.RoleID,
                            UserRoleName = role.UserRoleName,
                            ApplicationTypeID = role.ApplicationTypeID,
                            ApplicationType = role.ApplicationType,
                            InstituteID = role.InstituteID,
                            IsStudentRole = role.IsStudentRole,
                            Modules = new List<ModuleResponse1>()
                        };

                        // Add module, submodule, and functionalities if they don't exist yet
                        var moduleResponse1 = userRole.Modules.FirstOrDefault(m => m.ModuleID == module.ModuleID);
                        if (moduleResponse1 == null)
                        {
                            moduleResponse1 = new ModuleResponse1
                            {
                                ModuleID = module.ModuleID,
                                ModuleName = module.ModuleName,
                                Submodules = new List<SubmoduleResponse1>()
                            };
                            userRole.Modules.Add(moduleResponse1);
                        }

                        var submoduleResponse1 = moduleResponse1.Submodules.FirstOrDefault(sm => sm.SubModuleID == submodule.SubModuleID);
                        if (submoduleResponse1 == null)
                        {
                            submoduleResponse1 = new SubmoduleResponse1
                            {
                                SubModuleID = submodule.SubModuleID,
                                SubModuleName = submodule.SubModuleName,
                                Functionalities = new List<FunctionalityResponse1>() // Ensure it's initialized
                            };
                            moduleResponse1.Submodules.Add(submoduleResponse1);
                        }

                        // Check if Functionalities list is null, if so initialize it
                        if (submoduleResponse1.Functionalities == null)
                        {
                            submoduleResponse1.Functionalities = new List<FunctionalityResponse1>();
                        }

                        // Add functionality to the submodule's functionality list
                        submoduleResponse1.Functionalities.Add(new FunctionalityResponse1
                        {
                            FunctionalityID = functionality.FunctionalityID,
                            Functionality = functionality.Functionality
                        });

                        return userRole;
                    },
                    param: new { RoleID = request.RoleID, InstituteID = request.InstituteID },
                    splitOn: "ModuleID,SubModuleID,FunctionalityID"
                );

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return null;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public async Task<bool> DeleteEmployeeFromRoleAsync(DeleteEmployeeFromRoleRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            try
            {
                // SQL query to delete employee from role
                string query = @"
                    DELETE FROM tblUserRoleMapping
                    WHERE RoleID = @RoleID AND EmployeeID = @EmployeeID";

                int rowsAffected = await _connection.ExecuteAsync(query, request);

                return rowsAffected > 0; // Return true if the employee was removed successfully
            }
            catch (Exception)
            {
                return false;
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
