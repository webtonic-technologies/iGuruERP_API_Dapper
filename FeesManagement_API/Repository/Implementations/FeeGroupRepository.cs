using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;using FeesManagement_API.DTOs.Requests;


namespace FeesManagement_API.Repository.Implementations
{
    public class FeeGroupRepository : IFeeGroupRepository
    {
        private readonly IDbConnection _connection;

        public FeeGroupRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddUpdateFeeGroup(AddUpdateFeeGroupRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open(); // Ensure the connection is open
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var feeGroupId = request.FeeGroupID;

                    if (feeGroupId == 0)
                    {
                        var query = @"INSERT INTO tblFeeGroup (GroupName, FeeHeadID, FeeTenurityID, Fee, InstituteID) 
                              VALUES (@GroupName, @FeeHeadID, @FeeTenurityID, @Fee, @InstituteID);
                              SELECT CAST(SCOPE_IDENTITY() as int);";

                        feeGroupId = await _connection.ExecuteScalarAsync<int>(query, request, transaction);
                    }
                    else
                    {
                        var query = @"UPDATE tblFeeGroup 
                              SET GroupName = @GroupName, 
                                  FeeHeadID = @FeeHeadID, 
                                  FeeTenurityID = @FeeTenurityID, 
                                  Fee = @Fee, 
                                  InstituteID = @InstituteID
                              WHERE FeeGroupID = @FeeGroupID";

                        await _connection.ExecuteAsync(query, request, transaction);
                    }

                    // Handle tblFeeGroupClassSection
                    var deleteClassSectionsQuery = @"DELETE FROM tblFeeGroupClassSection WHERE FeeGroupID = @FeeGroupID";
                    await _connection.ExecuteAsync(deleteClassSectionsQuery, new { FeeGroupID = feeGroupId }, transaction);

                    var insertClassSectionQuery = @"INSERT INTO tblFeeGroupClassSection (FeeGroupID, ClassID, SectionID)
                                            VALUES (@FeeGroupID, @ClassID, @SectionID)";

                    if (request.ClassSections != null)
                    {
                        foreach (var section in request.ClassSections)
                        {
                            await _connection.ExecuteAsync(insertClassSectionQuery,
                                new { FeeGroupID = feeGroupId, section.ClassID, section.SectionID },
                                transaction);
                        }
                    }

                    // Handle tblFeeGroupCollection and related tables based on FeeTenurityID
                    var deleteFeeCollectionsQuery = @"DELETE FROM tblFeeGroupCollection WHERE FeeGroupID = @FeeGroupID";
                    await _connection.ExecuteAsync(deleteFeeCollectionsQuery, new { FeeGroupID = feeGroupId }, transaction);

                    if (request.FeeCollections != null)
                    {
                        foreach (var collection in request.FeeCollections)
                        {
                            var insertFeeCollectionQuery = @"INSERT INTO tblFeeGroupCollection (FeeGroupID, FeeTenurityID)
                                                     VALUES (@FeeGroupID, @FeeTenurityID);
                                                     SELECT CAST(SCOPE_IDENTITY() as int);";

                            var feeCollectionId = await _connection.ExecuteScalarAsync<int>(insertFeeCollectionQuery,
                                new { FeeGroupID = feeGroupId, collection.FeeTenurityID },
                                transaction);

                            if (collection.FeeTenurityID == 1 && collection.TenuritySingle != null)
                            {
                                var insertSingleQuery = @"INSERT INTO tblTenuritySingle (FeeCollectionID, Amount)
                                                  VALUES (@FeeCollectionID, @Amount)";
                                await _connection.ExecuteAsync(insertSingleQuery,
                                    new { FeeCollectionID = feeCollectionId, collection.TenuritySingle.Amount },
                                    transaction);
                            }
                            else if (collection.FeeTenurityID == 2 && collection.TenurityTerm != null)
                            {
                                var insertTermQuery = @"INSERT INTO tblTenurityTerm (FeeCollectionID, TermName, Amount, DueDate)
                                                VALUES (@FeeCollectionID, @TermName, @Amount, @DueDate)";
                                foreach (var term in collection.TenurityTerm.Terms)
                                {
                                    await _connection.ExecuteAsync(insertTermQuery,
                                        new { FeeCollectionID = feeCollectionId, term.TermName, term.Amount, term.DueDate },
                                        transaction);
                                }
                            }
                            else if (collection.FeeTenurityID == 3 && collection.TenurityMonthly != null)
                            {
                                var insertMonthQuery = @"INSERT INTO tblTenurityMonthly (FeeCollectionID, Month, Amount)
                                                 VALUES (@FeeCollectionID, @Month, @Amount)";
                                foreach (var month in collection.TenurityMonthly.Months)
                                {
                                    await _connection.ExecuteAsync(insertMonthQuery,
                                        new { FeeCollectionID = feeCollectionId, month.Month, month.Amount },
                                        transaction);
                                }
                            }
                        }
                    }

                    transaction.Commit();
                    return feeGroupId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _connection.Close(); // Ensure the connection is closed
                }
            }
        }


        public async Task<IEnumerable<FeeGroupResponse>> GetAllFeeGroups(GetAllFeeGroupRequest request)
    {
        var query = @"
            SELECT fg.FeeGroupID, fg.GroupName, fg.FeeHeadID, fh.FeeHead, fg.FeeTenurityID, ft.FeeTenurityType
            FROM tblFeeGroup fg
            INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
            INNER JOIN tblFeeTenurityMaster ft ON fg.FeeTenurityID = ft.FeeTenurityID
            LEFT JOIN tblFeeGroupClassSection fgc ON fg.FeeGroupID = fgc.FeeGroupID
            WHERE fg.FeeHeadID = @FeeHeadID AND fg.InstituteID = @InstituteID
            GROUP BY fg.FeeGroupID, fg.GroupName, fg.FeeHeadID, fh.FeeHead, fg.FeeTenurityID, ft.FeeTenurityType
            ORDER BY fg.FeeGroupID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var result = await _connection.QueryAsync<FeeGroupResponse>(query, new
        {
            FeeHeadID = request.FeeHeadID,
            InstituteID = request.InstituteID,
            Offset = (request.PageNumber - 1) * request.PageSize,
            PageSize = request.PageSize
        });

        return result.ToList();
    }

        public async Task<FeeGroupResponse> GetFeeGroupById(int feeGroupID)
        {
            var query = @"SELECT FeeGroupID, GroupName, FeeHeadID, FeeTenurityID, Fee, InstituteID 
                          FROM tblFeeGroup
                          WHERE FeeGroupID = @FeeGroupID";

            return await _connection.QueryFirstOrDefaultAsync<FeeGroupResponse>(query, new { FeeGroupID = feeGroupID });
        }

        public async Task<int> UpdateFeeGroupStatus(int feeGroupID)
        {
            string query = "UPDATE tblFeeGroup SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE FeeGroupID = @FeeGroupID";
            return await _connection.ExecuteAsync(query, new { FeeGroupID = feeGroupID });
        }

    }
}
