using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FeesManagement_API.DTOs.ServiceResponse;


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

                    var insertRuleQuery = @"INSERT INTO tblConcessionRules (ConcessionGroupID, FeeHeadID, Amount, FeeTenurityID, STMTenurityID, FeeCollectionID, InstituteID)
                                    VALUES (@ConcessionGroupID, @FeeHeadID, @Amount, @FeeTenurityID, @STMTenurityID, @FeeCollectionID, @InstituteID)";

                    if (request.ConcessionRules != null)
                    {
                        foreach (var rule in request.ConcessionRules)
                        {
                            await _connection.ExecuteAsync(insertRuleQuery,
                                new
                                {
                                    ConcessionGroupID = concessionGroupId,
                                    rule.FeeHeadID,
                                    rule.Amount,
                                    rule.FeeTenurityID,
                                    rule.STMTenurityID,
                                    rule.FeeCollectionID,
                                    request.InstituteID
                                },
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


        //public async Task<IEnumerable<ConcessionResponse>> GetAllConcessions(GetAllConcessionRequest request)
        //{
        //    var query = @"
        //SELECT 
        //    cg.ConcessionGroupID, 
        //    cg.ConcessionGroupType, 
        //    cg.IsAmount, 
        //    cg.IsPercentage, 
        //    cg.InstituteID,
        //    cr.ConcessionRulesID,
        //    cr.FeeHeadID,
        //    cr.Amount,
        //    cr.FeeTenurityID,
        //    cr.STMTenurityID,
        //    cr.FeeCollectionID,
        //    fh.FeeHead,
        //    CASE 
        //        WHEN cr.FeeTenurityID = 1 THEN 'Single'
        //        WHEN cr.FeeTenurityID = 2 THEN tt.TermName
        //        WHEN cr.FeeTenurityID = 3 THEN STRING_AGG(tm.Month, ', ') WITHIN GROUP (ORDER BY tm.Month)
        //        ELSE ''
        //    END AS FeeTenure
        //FROM tblConcessionGroup cg
        //LEFT JOIN tblConcessionRules cr ON cg.ConcessionGroupID = cr.ConcessionGroupID
        //LEFT JOIN tblFeeHead fh ON cr.FeeHeadID = fh.FeeHeadID
        //LEFT JOIN tblTenurityTerm tt ON cr.FeeCollectionID = tt.FeeCollectionID AND cr.FeeTenurityID = 2
        //LEFT JOIN tblTenurityMonthly tm ON cr.FeeCollectionID = tm.FeeCollectionID AND cr.FeeTenurityID = 3
        //WHERE cg.InstituteID = @InstituteID And cg.IsActive = 1
        //GROUP BY 
        //    cg.ConcessionGroupID, 
        //    cg.ConcessionGroupType, 
        //    cg.IsAmount, 
        //    cg.IsPercentage, 
        //    cg.InstituteID,
        //    cr.ConcessionRulesID,
        //    cr.FeeHeadID,
        //    cr.Amount,
        //    cr.FeeTenurityID,
        //    cr.STMTenurityID,
        //    cr.FeeCollectionID,
        //    fh.FeeHead,
        //    tt.TermName
        //ORDER BY cg.ConcessionGroupID
        //OFFSET @PageSize * (@PageNumber - 1) ROWS
        //FETCH NEXT @PageSize ROWS ONLY";

        //    // Declare and initialize concessionGroups before using it
        //    var concessionGroups = new List<ConcessionResponse>();

        //    var result = await _connection.QueryAsync<ConcessionResponse, ConcessionRuleResponse, ConcessionResponse>(
        //        query,
        //        (concession, rule) =>
        //        {
        //            // Find existing concession group in the list
        //            var existingConcession = concessionGroups.FirstOrDefault(cg => cg.ConcessionGroupID == concession.ConcessionGroupID);
        //            if (existingConcession != null)
        //            {
        //                existingConcession.ConcessionRules.Add(rule);
        //                return existingConcession;
        //            }
        //            else
        //            {
        //                concession.ConcessionRules = new List<ConcessionRuleResponse> { rule };
        //                concessionGroups.Add(concession);
        //                return concession;
        //            }
        //        },
        //        new { request.InstituteID, request.PageNumber, request.PageSize },
        //        splitOn: "ConcessionRulesID"
        //    );

        //    return concessionGroups;
        //}


        public async Task<ServiceResponse<IEnumerable<ConcessionResponse>>> GetAllConcessions(GetAllConcessionRequest request)
        {
            var query = @"
            SELECT 
                cg.ConcessionGroupID, 
                cg.ConcessionGroupType, 
                cg.IsAmount, 
                cg.IsPercentage, 
                cg.InstituteID,
                cr.ConcessionRulesID,
                cr.FeeHeadID,
                cr.Amount,
                cr.FeeTenurityID,
                cr.STMTenurityID,
                cr.FeeCollectionID,
                fh.FeeHead,
                CASE 
                    WHEN cr.FeeTenurityID = 1 THEN 'Single'
                    WHEN cr.FeeTenurityID = 2 THEN tt.TermName
                    WHEN cr.FeeTenurityID = 3 THEN STRING_AGG(tm.Month, ', ') WITHIN GROUP(ORDER BY tm.Month)
                    ELSE ''
                END AS FeeTenure,
                CASE 
                    WHEN cg.IsActive = 1 THEN 'true'
                    ELSE 'false'
                END AS IsActive
            FROM tblConcessionGroup cg
            LEFT JOIN tblConcessionRules cr ON cg.ConcessionGroupID = cr.ConcessionGroupID
            LEFT JOIN tblFeeHead fh ON cr.FeeHeadID = fh.FeeHeadID
            LEFT JOIN tblTenurityTerm tt ON cr.FeeCollectionID = tt.FeeCollectionID AND cr.FeeTenurityID = 2
            LEFT JOIN tblTenurityMonthly tm ON cr.FeeCollectionID = tm.FeeCollectionID AND cr.FeeTenurityID = 3
            WHERE cg.InstituteID = @InstituteID 
            GROUP BY 
                cg.ConcessionGroupID, 
                cg.ConcessionGroupType, 
                cg.IsAmount, 
                cg.IsPercentage, 
                cg.InstituteID,
                cr.ConcessionRulesID,
                cr.FeeHeadID,
                cr.Amount,
                cr.FeeTenurityID,
                cr.STMTenurityID,
                cr.FeeCollectionID,
                fh.FeeHead,
                tt.TermName,
                cg.IsActive
            ORDER BY cg.ConcessionGroupID
            OFFSET @PageSize * (@PageNumber - 1) ROWS
            FETCH NEXT @PageSize ROWS ONLY;";

            var concessionGroups = new List<ConcessionResponse>();

            var result = await _connection.QueryAsync<ConcessionResponse, ConcessionRuleResponse, ConcessionResponse>(
                query,
                (concession, rule) =>
                {
                    var existingConcession = concessionGroups.FirstOrDefault(cg => cg.ConcessionGroupID == concession.ConcessionGroupID);
                    if (existingConcession != null)
                    {
                        existingConcession.ConcessionRules.Add(rule);
                        return existingConcession;
                    }
                    else
                    {
                        concession.ConcessionRules = new List<ConcessionRuleResponse> { rule };
                        concessionGroups.Add(concession);
                        return concession;
                    }
                },
                new { request.InstituteID, request.PageNumber, request.PageSize },
                splitOn: "ConcessionRulesID"
            );

            var totalCountQuery = @"
        SELECT COUNT(*) 
        FROM tblConcessionGroup cg
        LEFT JOIN tblConcessionRules cr ON cg.ConcessionGroupID = cr.ConcessionGroupID
        LEFT JOIN tblFeeHead fh ON cr.FeeHeadID = fh.FeeHeadID
        LEFT JOIN tblTenurityTerm tt ON cr.FeeCollectionID = tt.FeeCollectionID AND cr.FeeTenurityID = 2
        LEFT JOIN tblTenurityMonthly tm ON cr.FeeCollectionID = tm.FeeCollectionID AND cr.FeeTenurityID = 3
        WHERE cg.InstituteID = @InstituteID AND cg.IsActive = 1;";

            var totalCount = await _connection.ExecuteScalarAsync<int>(totalCountQuery, new { request.InstituteID });

            return new ServiceResponse<IEnumerable<ConcessionResponse>>(true, "Concession groups retrieved successfully", concessionGroups, 200, totalCount);
        }


        public async Task<ConcessionResponse> GetConcessionById(int concessionGroupID)
        {
            var query = @"
        SELECT 
            cg.ConcessionGroupID, 
            cg.ConcessionGroupType, 
            cg.IsAmount, 
            cg.IsPercentage, 
            cg.InstituteID,
            cr.ConcessionRulesID,
            cr.FeeHeadID,
            cr.Amount,
            cr.FeeTenurityID,
            cr.STMTenurityID,
            cr.FeeCollectionID,
            fh.FeeHead,
            CASE 
                WHEN cr.FeeTenurityID = 1 THEN 'Single'
                WHEN cr.FeeTenurityID = 2 THEN tt.TermName
                WHEN cr.FeeTenurityID = 3 THEN STRING_AGG(tm.Month, ', ') WITHIN GROUP (ORDER BY tm.Month)
                ELSE ''
            END AS FeeTenure
        FROM tblConcessionGroup cg
        LEFT JOIN tblConcessionRules cr ON cg.ConcessionGroupID = cr.ConcessionGroupID
        LEFT JOIN tblFeeHead fh ON cr.FeeHeadID = fh.FeeHeadID
        LEFT JOIN tblTenurityTerm tt ON cr.FeeCollectionID = tt.FeeCollectionID AND cr.FeeTenurityID = 2
        LEFT JOIN tblTenurityMonthly tm ON cr.FeeCollectionID = tm.FeeCollectionID AND cr.FeeTenurityID = 3
        WHERE cg.ConcessionGroupID = @ConcessionGroupID
        GROUP BY 
            cg.ConcessionGroupID, 
            cg.ConcessionGroupType, 
            cg.IsAmount, 
            cg.IsPercentage, 
            cg.InstituteID,
            cr.ConcessionRulesID,
            cr.FeeHeadID,
            cr.Amount,
            cr.FeeTenurityID,
            cr.STMTenurityID,
            cr.FeeCollectionID,
            fh.FeeHead,
            tt.TermName";

            var lookup = new Dictionary<int, ConcessionResponse>();

            var result = await _connection.QueryAsync<ConcessionResponse, ConcessionRuleResponse, ConcessionResponse>(
                query,
                (concession, rule) =>
                {
                    if (!lookup.TryGetValue(concession.ConcessionGroupID, out var concessionGroup))
                    {
                        concessionGroup = concession;
                        concessionGroup.ConcessionRules = new List<ConcessionRuleResponse>();
                        lookup.Add(concessionGroup.ConcessionGroupID, concessionGroup);
                    }

                    concessionGroup.ConcessionRules.Add(rule);
                    return concessionGroup;
                },
                new { ConcessionGroupID = concessionGroupID },
                splitOn: "ConcessionRulesID"
            );

            return lookup.Values.FirstOrDefault();
        }


        //public async Task<int> UpdateConcessionGroupStatus(int concessionGroupID)
        //{
        //    string query = "UPDATE tblConcessionGroup SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE ConcessionGroupID = @ConcessionGroupID";
        //    return await _connection.ExecuteAsync(query, new { ConcessionGroupID = concessionGroupID });
        //}

        //public async Task<int> UpdateConcessionGroupStatus(int concessionGroupID, string? inActiveReason)
        //{

        //    //string query = "UPDATE tblConcessionGroup SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE ConcessionGroupID = @ConcessionGroupID";

        //    string query = @"
        //    UPDATE tblConcessionGroup 
        //    SET 
        //        IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END,
        //        InActiveReason = CASE WHEN IsActive = 0 THEN @InActiveReason ELSE NULL END
        //    WHERE ConcessionGroupID = @ConcessionGroupID";

        //    return await _connection.ExecuteAsync(query, new { ConcessionGroupID = concessionGroupID, InActiveReason = inActiveReason });
        //}


        public async Task<int> UpdateConcessionGroupStatus(int concessionGroupID, string? inActiveReason)
        {
            string query = @"
        UPDATE tblConcessionGroup 
        SET 
            IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END,
            InActiveReason = CASE 
                                WHEN IsActive = 0 THEN NULL  -- Clear InActiveReason when activating
                                WHEN IsActive = 1 THEN @InActiveReason -- Set InActiveReason when deactivating
                                ELSE InActiveReason
                             END
        WHERE ConcessionGroupID = @ConcessionGroupID";

            return await _connection.ExecuteAsync(query, new { ConcessionGroupID = concessionGroupID, InActiveReason = inActiveReason });
        }

    }
}
