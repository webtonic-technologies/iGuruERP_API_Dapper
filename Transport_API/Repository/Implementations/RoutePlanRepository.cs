using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using System.Data;

namespace Transport_API.Repository.Implementations
{
    public class RoutePlanRepository : IRoutePlanRepository
    {
        private readonly IDbConnection _dbConnection;

        public RoutePlanRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateRoutePlan(RoutePlan routePlan)
        {
            string sql;
            if (routePlan.RoutePlanID == 0)
            {
                sql = @"INSERT INTO tblRoutePlan (RouteName, VehicleID, InstituteID) 
                        VALUES (@RouteName, @VehicleID, @InstituteID)";
            }
            else
            {
                sql = @"UPDATE tblRoutePlan SET RouteName = @RouteName, VehicleID = @VehicleID, InstituteID = @InstituteID 
                        WHERE RoutePlanID = @RoutePlanID";
            }

            var result = await _dbConnection.ExecuteAsync(sql, routePlan);
            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Route plan added/updated successfully", StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating route plan", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<ServiceResponse<IEnumerable<RoutePlan>>> GetAllRoutePlans(GetAllRoutePlanRequest request)
        {
            string countSql = @"SELECT COUNT(*) FROM tblRoutePlan";
            int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql);

            string sql = @"SELECT * FROM tblRoutePlan ORDER BY RoutePlanID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var routePlans = await _dbConnection.QueryAsync<RoutePlan>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (routePlans.Any())
            {
                return new ServiceResponse<IEnumerable<RoutePlan>>(true, "Records Found", routePlans, StatusCodes.Status200OK, totalCount);
            }
            else
            {
                return new ServiceResponse<IEnumerable<RoutePlan>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<RoutePlan>> GetRoutePlanById(int RoutePlanID)
        {
            string sql = @"SELECT * FROM tblRoutePlan WHERE RoutePlanId = @RoutePlanID";
            var routePlan = await _dbConnection.QueryFirstOrDefaultAsync<RoutePlan>(sql, new { RoutePlanID = RoutePlanID });

            if (routePlan != null)
            {
                return new ServiceResponse<RoutePlan>(true, "Record Found", routePlan, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<RoutePlan>(false, "Record Not Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateRoutePlanStatus(int RoutePlanID)
        {
            string sql = @"UPDATE tblRoutePlan SET IsActive = @IsActive WHERE RoutePlanID = @RoutePlanID";
            var result = await _dbConnection.ExecuteAsync(sql, new { RoutePlanID = RoutePlanID });

            if (result > 0)
            {
                return new ServiceResponse<bool>(true, "Status Updated Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<bool>(false, "Status Update Failed", false, StatusCodes.Status400BadRequest);
            }
        }
    }
}
