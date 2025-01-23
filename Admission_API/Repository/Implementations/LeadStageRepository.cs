using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Admission_API.Repository.Implementations
{
    public class LeadStageRepository : ILeadStageRepository
    {
        private readonly IDbConnection _dbConnection;

        public LeadStageRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateLeadStage(List<LeadStage> request)
        {
            var resultMessage = string.Empty;

            foreach (var leadStage in request)
            {
                var query = leadStage.LeadStageID == 0 ?
                    @"INSERT INTO tblLeadStageMaster (LeadStage, ColorCode, IsActive, InstituteID) 
              VALUES (@StageName, @ColorCode, @IsActive, @InstituteID)" :
                    @"UPDATE tblLeadStageMaster 
              SET LeadStage = @StageName, ColorCode = @ColorCode, IsActive = @IsActive, InstituteID = @InstituteID 
              WHERE LeadStageID = @LeadStageID";

                var parameters = new
                {
                    LeadStageID = leadStage.LeadStageID,
                    StageName = leadStage.StageName,
                    ColorCode = leadStage.ColorCode,
                    IsActive = leadStage.IsActive,
                    InstituteID = leadStage.InstituteID
                };

                var result = await _dbConnection.ExecuteAsync(query, parameters);
                resultMessage += result > 0 ? "Success " : "Failure ";
            }

            return new ServiceResponse<string>(true, "Lead Stages Added/Updated Successfully", resultMessage, 200);
        }
         

        public async Task<ServiceResponse<List<LeadStage>>> GetAllLeadStages(GetAllRequest request)
        {
            string query = @"
            SELECT LeadStageID, LeadStage AS StageName, ColorCode, IsActive, InstituteID, IsDefault
            FROM tblLeadStageMaster 
            WHERE IsDefault = 1
            UNION ALL
            SELECT LeadStageID, LeadStage AS StageName, ColorCode, IsActive, InstituteID, IsDefault
            FROM tblLeadStageMaster 
            WHERE InstituteID = @InstituteID AND IsDefault = 0 AND IsActive = 1
            ORDER BY LeadStageID
            OFFSET @PageNumber ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            var result = (await _dbConnection.QueryAsync<LeadStage>(query, new
            {
                InstituteID = request.InstituteID,  // Pass the InstituteID from the request
                PageNumber = (request.PageNumber - 1) * request.PageSize,
                request.PageSize
            })).ToList();

            // Merged total count query
            var totalCountQuery = @"
            SELECT COUNT(*) 
            FROM tblLeadStageMaster 
            WHERE IsDefault = 1
            OR (InstituteID = @InstituteID AND IsDefault = 0 AND IsActive = 1)";

            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(
                totalCountQuery,
                new { InstituteID = request.InstituteID }
            );

            return new ServiceResponse<List<LeadStage>>(true, "Operation Successful", result, 200, totalCount);
        } 

        public async Task<ServiceResponse<LeadStage>> GetLeadStageById(int leadStageID)
        {
            var query = "SELECT LeadStageID, LeadStage AS StageName, ColorCode, InstituteID, IsDefault, IsActive FROM tblLeadStageMaster WHERE LeadStageID = @LeadStageID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<LeadStage>(query, new { LeadStageID = leadStageID });
            return new ServiceResponse<LeadStage>(true, "Operation Successful", result, 200);
        }

        public async Task<ServiceResponse<bool>> UpdateLeadStageStatus(int leadStageID)
        {
            try
            {
                // Check if the LeadStage is marked as default
                string checkQuery = "SELECT COUNT(*) FROM tblLeadStageMaster WHERE LeadStageID = @LeadStageID AND IsDefault = 1";
                int isDefaultCount = await _dbConnection.ExecuteScalarAsync<int>(checkQuery, new { LeadStageID = leadStageID });

                if (isDefaultCount > 0)
                {
                    return new ServiceResponse<bool>(false, "Cannot update the status of a LeadStage that is marked as default.", false, 400);
                }

                // If not default, proceed to toggle the IsActive status
                var query = "UPDATE tblLeadStageMaster SET IsActive = ~IsActive WHERE LeadStageID = @LeadStageID";
                var result = await _dbConnection.ExecuteAsync(query, new { LeadStageID = leadStageID });

                if (result > 0)
                {
                    return new ServiceResponse<bool>(true, "LeadStage status updated successfully.", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Failed to update LeadStage status.", false, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

    }
}
