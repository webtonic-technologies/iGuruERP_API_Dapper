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

        public async Task<ServiceResponse<string>> SetRolePermission(SetRolePermissionRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string roleQuery = @"INSERT INTO [tblUserRole] (UserRole, IsActive, InstituteID) 
                                         VALUES (@UserRole, @IsActive, @InstituteID);
                                         SELECT CAST(SCOPE_IDENTITY() as int)";
                    int roleId = await _connection.QuerySingleAsync<int>(roleQuery, request.Role, transaction);

                    string mappingQuery = @"INSERT INTO [tblUserRoleMapping] (RoleID, EmployeeID) 
                                            VALUES (@RoleID, @EmployeeID)";
                    foreach (var mapping in request.UserRoleMappings)
                    {
                        mapping.RoleID = roleId;
                        await _connection.ExecuteAsync(mappingQuery, mapping, transaction);
                    }

                    transaction.Commit();
                    return new ServiceResponse<string>(true, "Permission Set Successfully", "Success", 201);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
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

        public async Task<ServiceResponse<List<UserRoleWithPermissionsResponse>>> GetAllUserRoles()
        {
            try
            {
                string sql = @"SELECT ur.RoleID, ur.UserRole, ur.IsActive, ur.InstituteID, urm.EmployeeID 
                               FROM [tblUserRole] ur
                               LEFT JOIN [tblUserRoleMapping] urm ON ur.RoleID = urm.RoleID";

                var roleMappings = await _connection.QueryAsync<Role, int, UserRoleWithPermissionsResponse>(
                    sql,
                    (userRole, employeeID) => new UserRoleWithPermissionsResponse { RoleID = userRole.RoleID, UserRole = userRole.UserRole, IsActive = userRole.IsActive, InstituteID = userRole.InstituteID, EmployeeIDs = new List<int> { employeeID } },
                    splitOn: "EmployeeID"
                );

                var result = roleMappings.GroupBy(rm => new { rm.RoleID, rm.UserRole, rm.IsActive, rm.InstituteID }).Select(g => new UserRoleWithPermissionsResponse
                {
                    RoleID = g.Key.RoleID,
                    UserRole = g.Key.UserRole,
                    IsActive = g.Key.IsActive,
                    InstituteID = g.Key.InstituteID,
                    EmployeeIDs = g.SelectMany(rm => rm.EmployeeIDs).ToList()
                }).ToList();

                return new ServiceResponse<List<UserRoleWithPermissionsResponse>>(true, "Roles Retrieved Successfully", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<UserRoleWithPermissionsResponse>>(false, ex.Message, new List<UserRoleWithPermissionsResponse>(), 500);
            }
        }
    }
}
