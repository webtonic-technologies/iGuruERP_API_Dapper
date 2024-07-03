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
    public class NumberSchemeRepository : INumberSchemeRepository
    {
        private readonly IDbConnection _dbConnection;

        public NumberSchemeRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateNumberScheme(NumberScheme request)
        {
            var query = request.SchemeID == 0 ?
                @"INSERT INTO tblScheme (SchemeTypeID, FromDate, ToDate, Suffix, Prefix, StartingNumber, Padding) 
                  VALUES (@SchemeTypeID, @FromDate, @ToDate, @Suffix, @Prefix, @StartingNumber, @Padding)" :
                @"UPDATE tblScheme SET SchemeTypeID = @SchemeTypeID, FromDate = @FromDate, ToDate = @ToDate, 
                  Suffix = @Suffix, Prefix = @Prefix, StartingNumber = @StartingNumber, Padding = @Padding 
                  WHERE SchemeID = @SchemeID";

            var parameters = new
            {
                SchemeID = request.SchemeID,
                SchemeTypeID = request.SchemeTypeID,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Suffix = request.Suffix,
                Prefix = request.Prefix,
                StartingNumber = request.StartingNumber,
                Padding = request.Padding
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }

        public async Task<ServiceResponse<List<NumberScheme>>> GetAllNumberSchemes(GetAllRequest request)
        {
            var query = @"SELECT SchemeID, SchemeTypeID, FromDate, ToDate, Suffix, Prefix, StartingNumber, Padding 
                          FROM tblScheme 
                          ORDER BY SchemeID 
                          OFFSET @PageNumber ROWS 
                          FETCH NEXT @PageSize ROWS ONLY";
            var result = (await _dbConnection.QueryAsync<NumberScheme>(query, new { PageNumber = (request.PageNumber - 1) * request.PageSize, request.PageSize })).ToList();
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblScheme");
            return new ServiceResponse<List<NumberScheme>>(true, "Operation Successful", result, 200, totalCount);
        }

        public async Task<ServiceResponse<NumberScheme>> GetNumberSchemeById(int schemeID)
        {
            var query = "SELECT SchemeID, SchemeTypeID, FromDate, ToDate, Suffix, Prefix, StartingNumber, Padding FROM tblScheme WHERE SchemeID = @SchemeID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<NumberScheme>(query, new { SchemeID = schemeID });
            return new ServiceResponse<NumberScheme>(true, "Operation Successful", result, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteNumberScheme(int schemeID)
        {
            var query = "DELETE FROM tblScheme WHERE SchemeID = @SchemeID";
            var result = await _dbConnection.ExecuteAsync(query, new { SchemeID = schemeID });
            return new ServiceResponse<bool>(true, "Operation Successful", result > 0, 200);
        }
    }
}
