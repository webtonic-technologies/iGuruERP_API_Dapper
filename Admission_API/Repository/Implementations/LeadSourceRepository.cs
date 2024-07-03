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
    public class LeadSourceRepository : ILeadSourceRepository
    {
        private readonly IDbConnection _dbConnection;

        public LeadSourceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateLeadSource(LeadSource request)
        {
            var query = request.LeadSourceID == 0 ?
                @"INSERT INTO tblLeadSource (LeadSource, IsActive) VALUES (@SourceName, @IsActive)" :
                @"UPDATE tblLeadSource SET LeadSource = @SourceName, IsActive = @IsActive WHERE LeadSourceID = @LeadSourceID";

            var parameters = new
            {
                LeadSourceID = request.LeadSourceID,
                SourceName = request.SourceName,
                IsActive = request.IsActive
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }

        public async Task<ServiceResponse<List<LeadSource>>> GetAllLeadSources(GetAllRequest request)
        {
            var query = @"SELECT LeadSourceID, LeadSource AS SourceName, IsActive 
                          FROM tblLeadSource 
                          ORDER BY LeadSourceID 
                          OFFSET @PageNumber ROWS 
                          FETCH NEXT @PageSize ROWS ONLY";
            var result = (await _dbConnection.QueryAsync<LeadSource>(query, new { PageNumber = (request.PageNumber - 1) * request.PageSize, request.PageSize })).ToList();
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblLeadSource");
            return new ServiceResponse<List<LeadSource>>(true, "Operation Successful", result, 200, totalCount);
        }

        public async Task<ServiceResponse<LeadSource>> GetLeadSourceById(int leadSourceID)
        {
            var query = "SELECT LeadSourceID, LeadSource AS SourceName, IsActive FROM tblLeadSource WHERE LeadSourceID = @LeadSourceID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<LeadSource>(query, new { LeadSourceID = leadSourceID });
            return new ServiceResponse<LeadSource>(true, "Operation Successful", result, 200);
        }

        public async Task<ServiceResponse<bool>> UpdateLeadSourceStatus(int leadSourceID)
        {
            var query = "UPDATE tblLeadSource SET IsActive = ~IsActive WHERE LeadSourceID = @LeadSourceID";
            var result = await _dbConnection.ExecuteAsync(query, new { LeadSourceID = leadSourceID });
            return new ServiceResponse<bool>(true, "Operation Successful", result > 0, 200);
        }
    }
}
