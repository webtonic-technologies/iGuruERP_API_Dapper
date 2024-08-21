using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FeesManagement_API.Repository.Implementations
{
    public class NumberSchemeRepository : INumberSchemeRepository
    {
        private readonly IDbConnection _connection;

        public NumberSchemeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddUpdateNumberScheme(AddUpdateNumberSchemeRequest request)
        {
            if (request.NumberSchemeID == 0)
            {
                var query = @"INSERT INTO tblNumberScheme (SchemeTypeID, FromDate, ToDate, Prefix, Suffix, StartingNumber, Padding, InstituteID, IsActive) 
                              VALUES (@SchemeTypeID, @FromDate, @ToDate, @Prefix, @Suffix, @StartingNumber, @Padding, @InstituteID, 1);
                              SELECT CAST(SCOPE_IDENTITY() as int);";
                return await _connection.ExecuteScalarAsync<int>(query, request);
            }
            else
            {
                var query = @"UPDATE tblNumberScheme 
                              SET SchemeTypeID = @SchemeTypeID, FromDate = @FromDate, ToDate = @ToDate, 
                                  Prefix = @Prefix, Suffix = @Suffix, StartingNumber = @StartingNumber, 
                                  Padding = @Padding, InstituteID = @InstituteID 
                              WHERE NumberSchemeID = @NumberSchemeID";
                return await _connection.ExecuteAsync(query, request);
            }
        }

        public async Task<IEnumerable<NumberSchemeResponse>> GetAllNumberSchemes(GetAllNumberSchemesRequest request)
        {
            var query = @"SELECT * FROM tblNumberScheme 
                          WHERE InstituteID = @InstituteID 
                          ORDER BY NumberSchemeID 
                          OFFSET @PageSize * (@PageNumber - 1) ROWS 
                          FETCH NEXT @PageSize ROWS ONLY";

            return await _connection.QueryAsync<NumberSchemeResponse>(query, request);
        }

        public async Task<NumberSchemeResponse> GetNumberSchemeById(int numberSchemeID)
        {
            var query = @"SELECT * FROM tblNumberScheme WHERE NumberSchemeID = @NumberSchemeID";
            return await _connection.QueryFirstOrDefaultAsync<NumberSchemeResponse>(query, new { NumberSchemeID = numberSchemeID });
        }

        
        public async Task<int> UpdateNumberSchemeStatus(int numberSchemeID)
        {
            var query = @"UPDATE tblNumberScheme 
                          SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END 
                          WHERE NumberSchemeID = @NumberSchemeID";
            return await _connection.ExecuteAsync(query, new { NumberSchemeID = numberSchemeID });
        }
    }
}
