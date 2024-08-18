using Configuration.DTOs.Requests;
using Configuration.DTOs.Responses;
using Configuration.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Configuration.Repository.Implementations
{
    public class OfferRepository : IOfferRepository
    {
        private readonly IDbConnection _connection;

        public OfferRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddUpdateOffer(AddUpdateOfferRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var offerID = request.OfferID;

                    if (offerID == 0)
                    {
                        var query = @"INSERT INTO tblOffer (OfferName, AcademicYear, OpeningDate, ClosingDate, StudentTypeID, isAmount, isPercentage, Amount, IsActive, InstituteID) 
                                      VALUES (@OfferName, @AcademicYear, @OpeningDate, @ClosingDate, @StudentTypeID, @isAmount, @isPercentage, @Amount, @IsActive, @InstituteID);
                                      SELECT CAST(SCOPE_IDENTITY() as int);";

                        offerID = await _connection.ExecuteScalarAsync<int>(query, request, transaction);
                    }
                    else
                    {
                        var query = @"UPDATE tblOffer 
                                      SET OfferName = @OfferName, 
                                          AcademicYear = @AcademicYear, 
                                          OpeningDate = @OpeningDate, 
                                          ClosingDate = @ClosingDate,
                                          StudentTypeID = @StudentTypeID,
                                          isAmount = @isAmount,
                                          isPercentage = @isPercentage,
                                          Amount = @Amount,
                                          IsActive = @IsActive,
                                          InstituteID = @InstituteID
                                      WHERE OfferID = @OfferID";

                        await _connection.ExecuteAsync(query, request, transaction);
                    }

                    // Handle Fee Head Mapping
                    var deleteFeeHeadMappingQuery = @"DELETE FROM tblOfferFeeHeadMapping WHERE OfferID = @OfferID";
                    await _connection.ExecuteAsync(deleteFeeHeadMappingQuery, new { OfferID = offerID }, transaction);

                    var insertFeeHeadMappingQuery = @"INSERT INTO tblOfferFeeHeadMapping (OfferID, FeeHeadID)
                                                      VALUES (@OfferID, @FeeHeadID)";

                    if (request.OfferFeeHeadMappings != null)
                    {
                        foreach (var feeHeadMapping in request.OfferFeeHeadMappings)
                        {
                            await _connection.ExecuteAsync(insertFeeHeadMappingQuery,
                                new { OfferID = offerID, feeHeadMapping.FeeHeadID },
                                transaction);
                        }
                    }

                    // Handle Fee Tenure Mapping
                    var deleteFeeTenureMappingQuery = @"DELETE FROM tblOfferFeeTenureMapping WHERE OfferID = @OfferID";
                    await _connection.ExecuteAsync(deleteFeeTenureMappingQuery, new { OfferID = offerID }, transaction);

                    var insertFeeTenureMappingQuery = @"INSERT INTO tblOfferFeeTenureMapping (OfferID, FeeTenurityID)
                                                        VALUES (@OfferID, @FeeTenurityID)";

                    if (request.OfferFeeTenureMappings != null)
                    {
                        foreach (var feeTenureMapping in request.OfferFeeTenureMappings)
                        {
                            await _connection.ExecuteAsync(insertFeeTenureMappingQuery,
                                new { OfferID = offerID, feeTenureMapping.FeeTenurityID },
                                transaction);
                        }
                    }

                    // Handle Class Section Mapping
                    var deleteClassSectionMappingQuery = @"DELETE FROM tblOfferClassSectionMapping WHERE OfferID = @OfferID";
                    await _connection.ExecuteAsync(deleteClassSectionMappingQuery, new { OfferID = offerID }, transaction);

                    var insertClassSectionMappingQuery = @"INSERT INTO tblOfferClassSectionMapping (OfferID, ClassID, SectionID)
                                                          VALUES (@OfferID, @ClassID, @SectionID)";

                    if (request.OfferClassSectionMappings != null)
                    {
                        foreach (var classSectionMapping in request.OfferClassSectionMappings)
                        {
                            await _connection.ExecuteAsync(insertClassSectionMappingQuery,
                                new { OfferID = offerID, classSectionMapping.ClassID, classSectionMapping.SectionID },
                                transaction);
                        }
                    }

                    transaction.Commit();
                    return offerID;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<OfferResponse>> GetAllOffers(GetAllOffersRequest request)
        {
            var query = @"SELECT o.OfferID, o.OfferName, o.AcademicYear, o.OpeningDate, o.ClosingDate, o.isAmount, o.isPercentage, o.Amount, o.IsActive
                          FROM tblOffer o
                          WHERE o.InstituteID = @InstituteID
                          ORDER BY o.OfferID
                          OFFSET @PageSize * (@PageNumber - 1) ROWS
                          FETCH NEXT @PageSize ROWS ONLY";

            return await _connection.QueryAsync<OfferResponse>(query, request);
        }

        public async Task<OfferResponse> GetOfferById(int offerID)
        {
            var query = @"SELECT OfferID, OfferName, AcademicYear, OpeningDate, ClosingDate, isAmount, isPercentage, Amount, IsActive 
                          FROM tblOffer 
                          WHERE OfferID = @OfferID";

            return await _connection.QueryFirstOrDefaultAsync<OfferResponse>(query, new { OfferID = offerID });
        }

        public async Task<int> DeleteOffer(int offerID)
        {
            var query = @"UPDATE tblOffer 
                          SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END 
                          WHERE OfferID = @OfferID";

            return await _connection.ExecuteAsync(query, new { OfferID = offerID });
        }
    }
}
