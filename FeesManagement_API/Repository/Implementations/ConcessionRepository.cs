using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FeesManagement_API.Repository.Implementations
{
    public class ConcessionRepository : IConcessionRepository
    {
        private readonly IDbConnection _connection;

        public ConcessionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddUpdateConcession(AddUpdateConcessionRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var concessionGroupId = request.ConcessionGroupID;

                    if (concessionGroupId == 0)
                    {
                        var query = @"INSERT INTO tblConcessionGroup (ConcessionGroupType, IsAmount, IsPercentage, InstituteID) 
                              VALUES (@ConcessionGroupType, @IsAmount, @IsPercentage, @InstituteID);
                              SELECT CAST(SCOPE_IDENTITY() as int);";

                        concessionGroupId = await _connection.ExecuteScalarAsync<int>(query, request, transaction);
                    }
                    else
                    {
                        var query = @"UPDATE tblConcessionGroup 
                              SET ConcessionGroupType = @ConcessionGroupType, 
                                  IsAmount = @IsAmount, 
                                  IsPercentage = @IsPercentage, 
                                  InstituteID = @InstituteID
                              WHERE ConcessionGroupID = @ConcessionGroupID";

                        await _connection.ExecuteAsync(query, request, transaction);
                    }

                    // Handle tblConcessionRules
                    var deleteRulesQuery = @"DELETE FROM tblConcessionRules WHERE ConcessionGroupID = @ConcessionGroupID";
                    await _connection.ExecuteAsync(deleteRulesQuery, new { ConcessionGroupID = concessionGroupId }, transaction);

                    var insertRuleQuery = @"INSERT INTO tblConcessionRules (ConcessionGroupID, FeeHeadID, Amount, InstituteID)
                                    VALUES (@ConcessionGroupID, @FeeHeadID, @Amount, @InstituteID)";

                    if (request.ConcessionRules != null)
                    {
                        foreach (var rule in request.ConcessionRules)
                        {
                            await _connection.ExecuteAsync(insertRuleQuery,
                                new { ConcessionGroupID = concessionGroupId, rule.FeeHeadID, rule.Amount, request.InstituteID },
                                transaction);
                        }
                    }

                    transaction.Commit();
                    return concessionGroupId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }


        public async Task<IEnumerable<ConcessionResponse>> GetAllConcessions(GetAllConcessionRequest request)
        {
            var query = @"SELECT cg.ConcessionGroupID, cg.ConcessionGroupType, cg.IsAmount, cg.IsPercentage, cg.InstituteID, 
                          cr.ConcessionRulesID, cr.FeeHeadID, cr.Amount
                          FROM tblConcessionGroup cg
                          LEFT JOIN tblConcessionRules cr ON cg.ConcessionGroupID = cr.ConcessionGroupID
                          WHERE cg.InstituteID = @InstituteID
                          ORDER BY cg.ConcessionGroupID
                          OFFSET @PageSize * (@PageNumber - 1) ROWS
                          FETCH NEXT @PageSize ROWS ONLY";

            var concessionGroups = await _connection.QueryAsync<ConcessionResponse, ConcessionRuleResponse, ConcessionResponse>(
                query,
                (concession, rule) =>
                {
                    concession.ConcessionRules = concession.ConcessionRules ?? new List<ConcessionRuleResponse>();
                    concession.ConcessionRules.Add(rule);
                    return concession;
                },
                new { request.InstituteID, request.PageNumber, request.PageSize },
                splitOn: "ConcessionRulesID"
            );

            return concessionGroups;
        }

        public async Task<ConcessionResponse> GetConcessionById(int concessionGroupID)
        {
            var query = @"SELECT cg.ConcessionGroupID, cg.ConcessionGroupType, cg.IsAmount, cg.IsPercentage, cg.InstituteID, 
                          cr.ConcessionRulesID, cr.FeeHeadID, cr.Amount
                          FROM tblConcessionGroup cg
                          LEFT JOIN tblConcessionRules cr ON cg.ConcessionGroupID = cr.ConcessionGroupID
                          WHERE cg.ConcessionGroupID = @ConcessionGroupID";

            var concession = await _connection.QueryFirstOrDefaultAsync<ConcessionResponse>(query, new { ConcessionGroupID = concessionGroupID });

            return concession;
        }

        public async Task<int> UpdateConcessionGroupStatus(int concessionGroupID)
        {
            string query = "UPDATE tblConcessionGroup SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE ConcessionGroupID = @ConcessionGroupID";
            return await _connection.ExecuteAsync(query, new { ConcessionGroupID = concessionGroupID });
        }

    }
}
