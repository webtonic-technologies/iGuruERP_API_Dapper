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

        public async Task<ServiceResponse<string>> AddUpdateLeadStage(LeadStage request)
        {
            var query = request.LeadStageID == 0 ?
                @"INSERT INTO tblLeadStageMaster (LeadStage, ColorCode, IsActive) VALUES (@StageName, @ColorCode, @IsActive)" :
                @"UPDATE tblLeadStageMaster SET LeadStage = @StageName, ColorCode = @ColorCode, IsActive = @IsActive WHERE LeadStageID = @LeadStageID";

            var parameters = new
            {
                LeadStageID = request.LeadStageID,
                StageName = request.StageName,
                ColorCode = request.ColorCode,
                IsActive = request.IsActive
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }

        public async Task<ServiceResponse<List<LeadStage>>> GetAllLeadStages(GetAllRequest request)
        {
            var query = @"SELECT LeadStageID, LeadStage AS StageName, ColorCode, IsActive 
                          FROM tblLeadStageMaster 
                          ORDER BY LeadStageID 
                          OFFSET @PageNumber ROWS 
                          FETCH NEXT @PageSize ROWS ONLY";
            var result = (await _dbConnection.QueryAsync<LeadStage>(query, new { PageNumber = (request.PageNumber - 1) * request.PageSize, request.PageSize })).ToList();
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblLeadStageMaster");
            return new ServiceResponse<List<LeadStage>>(true, "Operation Successful", result, 200, totalCount);
        }

        public async Task<ServiceResponse<LeadStage>> GetLeadStageById(int leadStageID)
        {
            var query = "SELECT LeadStageID, LeadStage AS StageName, ColorCode, IsActive FROM tblLeadStageMaster WHERE LeadStageID = @LeadStageID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<LeadStage>(query, new { LeadStageID = leadStageID });
            return new ServiceResponse<LeadStage>(true, "Operation Successful", result, 200);
        }

        public async Task<ServiceResponse<bool>> UpdateLeadStageStatus(int leadStageID)
        {
            var query = "UPDATE tblLeadStageMaster SET IsActive = ~IsActive WHERE LeadStageID = @LeadStageID";
            var result = await _dbConnection.ExecuteAsync(query, new { LeadStageID = leadStageID });
            return new ServiceResponse<bool>(true, "Operation Successful", result > 0, 200);
        }
    }
}
