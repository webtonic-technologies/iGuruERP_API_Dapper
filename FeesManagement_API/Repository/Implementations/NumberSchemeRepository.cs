using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
            // Parse the FromDate and ToDate from string to DateTime using the 'dd-MM-yyyy' format
            var fromDate = DateTime.ParseExact(request.FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var toDate = DateTime.ParseExact(request.ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            if (request.NumberSchemeID == 0)
            {
                var query = @"INSERT INTO tblNumberScheme (SchemeTypeID, FromDate, ToDate, Prefix, Suffix, StartingNumber, Padding, InstituteID, IsActive) 
                      VALUES (@SchemeTypeID, @FromDate, @ToDate, @Prefix, @Suffix, @StartingNumber, @Padding, @InstituteID, 1);
                      SELECT CAST(SCOPE_IDENTITY() as int);";
                return await _connection.ExecuteScalarAsync<int>(query, new
                {
                    request.SchemeTypeID,
                    FromDate = fromDate,
                    ToDate = toDate,
                    request.Prefix,
                    request.Suffix,
                    request.StartingNumber,
                    request.Padding,
                    request.InstituteID
                });
            }
            else
            {
                var query = @"UPDATE tblNumberScheme 
                      SET SchemeTypeID = @SchemeTypeID, FromDate = @FromDate, ToDate = @ToDate, 
                          Prefix = @Prefix, Suffix = @Suffix, StartingNumber = @StartingNumber, 
                          Padding = @Padding, InstituteID = @InstituteID 
                      WHERE NumberSchemeID = @NumberSchemeID";
                return await _connection.ExecuteAsync(query, new
                {
                    request.SchemeTypeID,
                    FromDate = fromDate,
                    ToDate = toDate,
                    request.Prefix,
                    request.Suffix,
                    request.StartingNumber,
                    request.Padding,
                    request.InstituteID,
                    request.NumberSchemeID
                });
            }
        }


        public async Task<IEnumerable<NumberSchemeResponse>> GetAllNumberSchemes(GetAllNumberSchemesRequest request)
        {
            var query = @"
        SELECT 
            ns.NumberSchemeID, 
            ns.SchemeTypeID, 
            st.SchemeType, 
            ns.FromDate, 
            ns.ToDate, 
            ns.Prefix, 
            ns.Suffix, 
            ns.StartingNumber, 
            ns.Padding, 
            ns.InstituteID, 
            ns.IsActive
        FROM tblNumberScheme ns
        LEFT JOIN tblSchemeType st ON ns.SchemeTypeID = st.SchemeTypeID
        WHERE ns.InstituteID = @InstituteID and ns.IsActive = 1
        ORDER BY ns.NumberSchemeID
        OFFSET @PageSize * (@PageNumber - 1) ROWS
        FETCH NEXT @PageSize ROWS ONLY";

            var numberSchemes = await _connection.QueryAsync<NumberSchemeResponse>(query, new
            {
                request.InstituteID,
                request.PageNumber,
                request.PageSize
            });

            // Format the dates and set the DateRange
            foreach (var scheme in numberSchemes)
            {
                scheme.FromDateFormatted = scheme.FromDate.ToString("dd-MM-yyyy");
                scheme.ToDateFormatted = scheme.ToDate.ToString("dd-MM-yyyy");
                scheme.DateRange = $"{scheme.FromDateFormatted} to {scheme.ToDateFormatted}";
            }

            return numberSchemes;
        }


        public async Task<NumberSchemeResponse> GetNumberSchemeById(int numberSchemeID)
        {
            var query = @"
        SELECT 
            ns.NumberSchemeID, 
            ns.SchemeTypeID, 
            st.SchemeType, 
            ns.FromDate, 
            ns.ToDate, 
            ns.Prefix, 
            ns.Suffix, 
            ns.StartingNumber, 
            ns.Padding, 
            ns.InstituteID, 
            ns.IsActive
        FROM tblNumberScheme ns
        LEFT JOIN tblSchemeType st ON ns.SchemeTypeID = st.SchemeTypeID
        WHERE ns.NumberSchemeID = @NumberSchemeID";

            var scheme = await _connection.QueryFirstOrDefaultAsync<NumberSchemeResponse>(query, new { NumberSchemeID = numberSchemeID });

            if (scheme != null)
            {
                // Format the dates and set the DateRange
                scheme.FromDateFormatted = scheme.FromDate.ToString("dd-MM-yyyy");
                scheme.ToDateFormatted = scheme.ToDate.ToString("dd-MM-yyyy");
                scheme.DateRange = $"{scheme.FromDateFormatted} to {scheme.ToDateFormatted}";
            }

            return scheme;
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
