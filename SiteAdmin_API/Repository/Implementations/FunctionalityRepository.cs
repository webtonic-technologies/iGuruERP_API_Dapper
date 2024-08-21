using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using System.Data;

namespace SiteAdmin_API.Repository.Implementations
{
    public class FunctionalityRepository : IFunctionalityRepository
    {
        private readonly IDbConnection _connection;

        public FunctionalityRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<bool>> UpdateFunctionality(UpdateFunctionalityRequest request)
        {
            try
            {
                string updateSql = @"
                    UPDATE tblFunctionality
                    SET Functionality = @Functionality,
                        SubModuleID = @SubModuleID,
                        IsActive = @IsActive
                    WHERE FunctionalityID = @FunctionalityID";

                int rowsAffected = await _connection.ExecuteAsync(updateSql, request);

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Functionality updated successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Failed to update functionality", false, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateFunctionalityStatus(int functionalityId)
        {
            try
            {
                // Toggle the IsActive status
                string query = "UPDATE tblFunctionality SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE FunctionalityID = @FunctionalityID";
                int rowsAffected = await _connection.ExecuteAsync(query, new { FunctionalityID = functionalityId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Functionality status updated successfully", true, 200);
                }

                return new ServiceResponse<bool>(false, "Failed to update functionality status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
