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

        public async Task<ServiceResponse<string>> AddUpdateLeadSource(List<LeadSource> request)
        {
            foreach (var leadSource in request)
            {
                var query = leadSource.LeadSourceID == 0 ?
                    @"INSERT INTO tblLeadSource (LeadSource, IsActive, InstituteID) 
              VALUES (@SourceName, @IsActive, @InstituteID)" :
                    @"UPDATE tblLeadSource 
              SET LeadSource = @SourceName, IsActive = @IsActive, InstituteID = @InstituteID 
              WHERE LeadSourceID = @LeadSourceID";

                var parameters = new
                {
                    LeadSourceID = leadSource.LeadSourceID,
                    SourceName = leadSource.SourceName,
                    IsActive = leadSource.IsActive,
                    InstituteID = leadSource.InstituteID
                };

                var result = await _dbConnection.ExecuteAsync(query, parameters);
                if (result <= 0)
                {
                    return new ServiceResponse<string>(false, "Failed to Add/Update LeadSource", null, 400);
                }
            }

            return new ServiceResponse<string>(true, "LeadSources Added/Updated Successfully", "Success", 200);
        }



        public async Task<ServiceResponse<List<LeadSourceResponse>>> GetAllLeadSources(GetAllRequest request)
        {
            var query = @"
            SELECT LeadSourceID, LeadSource AS SourceName, IsActive 
            FROM tblLeadSource 
            WHERE IsActive = 1 AND InstituteID = @InstituteID  -- Filter by InstituteID
            ORDER BY LeadSourceID 
            OFFSET @PageNumber ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            var result = (await _dbConnection.QueryAsync<LeadSourceResponse>(query, new
            {
                PageNumber = (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                InstituteID = request.InstituteID  // Include InstituteID in parameters
            })).ToList();

            // Count the total number of active lead sources for the given InstituteID
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM tblLeadSource WHERE IsActive = 1 AND InstituteID = @InstituteID",
                new { InstituteID = request.InstituteID }
            );

            return new ServiceResponse<List<LeadSourceResponse>>(true, "Operation Successful", result, 200, totalCount);
        }


        public async Task<ServiceResponse<LeadSourceResponse>> GetLeadSourceById(int leadSourceID)
        {
            var query = "SELECT LeadSourceID, LeadSource AS SourceName, IsActive FROM tblLeadSource WHERE LeadSourceID = @LeadSourceID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<LeadSourceResponse>(query, new { LeadSourceID = leadSourceID });
            return new ServiceResponse<LeadSourceResponse>(true, "Operation Successful", result, 200);
        }

        public async Task<ServiceResponse<bool>> UpdateLeadSourceStatus(int leadSourceID)
        {
            var query = "UPDATE tblLeadSource SET IsActive = ~IsActive WHERE LeadSourceID = @LeadSourceID";
            var result = await _dbConnection.ExecuteAsync(query, new { LeadSourceID = leadSourceID });
            return new ServiceResponse<bool>(true, "Operation Successful", result > 0, 200);
        }
    }
}
