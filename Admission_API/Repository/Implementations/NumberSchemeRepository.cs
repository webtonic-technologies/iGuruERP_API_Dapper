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
            // Convert FromDate and ToDate to DateTime objects, but store as strings (DD-MM-YYYY format)
            DateTime fromDate = DateTime.ParseExact(request.FromDate, "dd-MM-yyyy", null);
            DateTime toDate = DateTime.ParseExact(request.ToDate, "dd-MM-yyyy", null);

            var query = request.SchemeID == 0 ?
                @"INSERT INTO tblScheme (SchemeTypeID, FromDate, ToDate, Suffix, Prefix, StartingNumber, Padding, InstituteID) 
          VALUES (@SchemeTypeID, @FromDate, @ToDate, @Suffix, @Prefix, @StartingNumber, @Padding, @InstituteID)" :
                @"UPDATE tblScheme 
          SET SchemeTypeID = @SchemeTypeID, FromDate = @FromDate, ToDate = @ToDate, 
              Suffix = @Suffix, Prefix = @Prefix, StartingNumber = @StartingNumber, Padding = @Padding, InstituteID = @InstituteID 
          WHERE SchemeID = @SchemeID";

            var parameters = new
            {
                SchemeID = request.SchemeID,
                SchemeTypeID = request.SchemeTypeID,
                FromDate = fromDate,  // Now passing DateTime objects
                ToDate = toDate,      // Now passing DateTime objects
                Suffix = request.Suffix,
                Prefix = request.Prefix,
                StartingNumber = request.StartingNumber,
                Padding = request.Padding,
                InstituteID = request.InstituteID // Include InstituteID in the parameters
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }


        public async Task<ServiceResponse<List<NumberSchemeResponse>>> GetAllNumberSchemes(GetAllRequest request)
        {
            var query = @"
            SELECT 
                s.SchemeID, 
                s.SchemeTypeID, 
                CONVERT(VARCHAR(10), s.FromDate, 105) AS FromDate,   -- Format the FromDate to DD-MM-YYYY
                CONVERT(VARCHAR(10), s.ToDate, 105) AS ToDate,       -- Format the ToDate to DD-MM-YYYY
                s.Suffix, 
                s.Prefix, 
                s.StartingNumber, 
                s.Padding, 
                s.InstituteID,
                ast.SchemeType  -- Adding SchemeType from tblAdmissionSchemeType
            FROM tblScheme s
            INNER JOIN tblAdmissionSchemeType ast ON s.SchemeTypeID = ast.SchemeTypeID  -- JOIN with tblAdmissionSchemeType
            WHERE s.InstituteID = @InstituteID  -- Filter by InstituteID
            ORDER BY s.SchemeID 
            OFFSET @PageNumber ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            var result = (await _dbConnection.QueryAsync<NumberSchemeResponse>(query, new
            {
                PageNumber = (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                request.InstituteID // Include InstituteID in query parameters
            })).ToList();

            var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblScheme WHERE InstituteID = @InstituteID", new { request.InstituteID });

            return new ServiceResponse<List<NumberSchemeResponse>>(true, "Operation Successful", result, 200, totalCount);
        }



        public async Task<ServiceResponse<NumberSchemeResponse>> GetNumberSchemeById(int schemeID)
        {
            var query = @"SELECT
                        s.SchemeID, 
                        s.SchemeTypeID, 
                        CONVERT(VARCHAR(10), s.FromDate, 105) AS FromDate,   --Format the FromDate to DD-MM - YYYY
                        CONVERT(VARCHAR(10), s.ToDate, 105) AS ToDate,       --Format the ToDate to DD-MM - YYYY
                        s.Suffix, 
                        s.Prefix, 
                        s.StartingNumber, 
                        s.Padding, 
                        s.InstituteID,
                        ast.SchemeType-- Adding SchemeType from tblAdmissionSchemeType
                    FROM tblScheme s
                    INNER JOIN tblAdmissionSchemeType ast ON s.SchemeTypeID = ast.SchemeTypeID
                    WHERE s.SchemeID = @SchemeID";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<NumberSchemeResponse>(query, new { SchemeID = schemeID });
            return new ServiceResponse<NumberSchemeResponse>(true, "Operation Successful", result, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteNumberScheme(int schemeID)
        {
            var query = "DELETE FROM tblScheme WHERE SchemeID = @SchemeID";
            var result = await _dbConnection.ExecuteAsync(query, new { SchemeID = schemeID });
            return new ServiceResponse<bool>(true, "Operation Successful", result > 0, 200);
        }
    }
}
