using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class OptionalFeeRepository : IOptionalFeeRepository
    {
        private readonly IDbConnection _connection;

        public OptionalFeeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddUpdateOptionalFee(AddUpdateOptionalFeeRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    int optionalFeeId = 0;

                    foreach (var optionalFee in request.OptionalFees)
                    {
                        if (optionalFee.OptionalFeeID == 0)
                        {
                            var query = @"INSERT INTO tblOptionalFee (HeadName, ShortName, FeeAmount, InstituteID, IsActive) 
                                  VALUES (@HeadName, @ShortName, @FeeAmount, @InstituteID, @IsActive);
                                  SELECT CAST(SCOPE_IDENTITY() as int);";

                            optionalFeeId = await _connection.ExecuteScalarAsync<int>(query, new
                            {
                                optionalFee.HeadName,
                                optionalFee.ShortName,
                                optionalFee.FeeAmount,
                                request.InstituteID,  // Make sure InstituteID is passed from the request
                                optionalFee.IsActive
                            }, transaction);
                        }
                        else
                        {
                            var query = @"UPDATE tblOptionalFee 
                                  SET HeadName = @HeadName, 
                                      ShortName = @ShortName, 
                                      FeeAmount = @FeeAmount, 
                                      InstituteID = @InstituteID,
                                      IsActive = @IsActive
                                  WHERE OptionalFeeID = @OptionalFeeID";

                            await _connection.ExecuteAsync(query, new
                            {
                                optionalFee.HeadName,
                                optionalFee.ShortName,
                                optionalFee.FeeAmount,
                                request.InstituteID,  // Ensure InstituteID is correctly passed from the request
                                optionalFee.IsActive,
                                optionalFee.OptionalFeeID
                            }, transaction);
                        }
                    }

                    transaction.Commit();
                    return optionalFeeId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        _connection.Close();
                    }
                }
            }
        }


        public async Task<IEnumerable<OptionalFeeResponse>> GetAllOptionalFees(GetAllOptionalFeesRequest request)
        {
            var query = @"
                SELECT OptionalFeeID, HeadName, ShortName, FeeAmount, InstituteID, IsActive
                FROM tblOptionalFee
                WHERE InstituteID = @InstituteID
                ORDER BY OptionalFeeID
                OFFSET @PageSize * (@PageNumber - 1) ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            return await _connection.QueryAsync<OptionalFeeResponse>(query, new
            {
                InstituteID = request.InstituteID,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            });
        }

        public async Task<OptionalFeeResponse> GetOptionalFeeById(int optionalFeeID)
        {
            var query = @"SELECT OptionalFeeID, HeadName, ShortName, FeeAmount, InstituteID, IsActive 
                          FROM tblOptionalFee 
                          WHERE OptionalFeeID = @OptionalFeeID";

            return await _connection.QueryFirstOrDefaultAsync<OptionalFeeResponse>(query, new { OptionalFeeID = optionalFeeID });
        }

        public async Task<int> UpdateOptionalFeeStatus(int optionalFeeID)
        {
            var query = @"UPDATE tblOptionalFee 
                          SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END 
                          WHERE OptionalFeeID = @OptionalFeeID";

            return await _connection.ExecuteAsync(query, new { OptionalFeeID = optionalFeeID });
        }
    }
}
