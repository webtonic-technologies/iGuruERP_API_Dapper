using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Implementations
{
    public class StockEntryRepository : IStockEntryRepository
    {
        private readonly IDbConnection _connection;

        public StockEntryRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddUpdateStockEntry(AddUpdateStockEntryRequest request)
        {
            try
            {
                // Convert string dates to DateTime in 'DD-MM-YYYY' format
                DateTime expiryDate = DateTime.ParseExact(request.ExpiryDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime entryDate = DateTime.ParseExact(request.EntryDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                string query = request.StockID == 0
                    ? @"INSERT INTO tblStockEntry (ItemTypeID, MedicineName, Company, BatchCode, Diagnosis, Quantity, PricePerQuantity, ExpiryDate, EntryDate, DosageDetails, InstituteID, IsActive) 
               VALUES (@ItemTypeID, @MedicineName, @Company, @BatchCode, @Diagnosis, @Quantity, @PricePerQuantity, @ExpiryDate, @EntryDate, @DosageDetails, @InstituteID, @IsActive)"
                    : @"UPDATE tblStockEntry SET ItemTypeID = @ItemTypeID, MedicineName = @MedicineName, Company = @Company, 
               BatchCode = @BatchCode, Diagnosis = @Diagnosis, Quantity = @Quantity, PricePerQuantity = @PricePerQuantity, 
               ExpiryDate = @ExpiryDate, EntryDate = @EntryDate, DosageDetails = @DosageDetails, InstituteID = @InstituteID, IsActive = @IsActive 
               WHERE StockID = @StockID";

                // Execute the query with parsed DateTime values
                int result = await _connection.ExecuteAsync(query, new
                {
                    request.ItemTypeID,
                    request.MedicineName,
                    request.Company,
                    request.BatchCode,
                    request.Diagnosis,
                    request.Quantity,
                    request.PricePerQuantity,
                    ExpiryDate = expiryDate,  // Pass the DateTime value
                    EntryDate = entryDate,    // Pass the DateTime value
                    request.DosageDetails,
                    request.InstituteID,
                    request.IsActive,
                    request.StockID
                });

                if (result > 0)
                    return new ServiceResponse<string>(true, "Operation Successful", "Stock entry updated successfully", 200);
                else
                    return new ServiceResponse<string>(false, "Operation Failed", null, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<List<StockEntryResponse>>> GetAllStockEntries(GetAllStockEntriesRequest request)
        {
            try
            {
                // Parse StartDate and EndDate strings into DateTime if they are not null
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (!string.IsNullOrEmpty(request.StartDate))
                {
                    startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(request.EndDate))
                {
                    endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                // Build dynamic count query based on filters
                string countSql = @"
        SELECT COUNT(*) 
        FROM tblStockEntry 
        WHERE IsActive = 1 
          AND InstituteID = @InstituteID 
          AND (@StartDate IS NULL OR EntryDate >= @StartDate) 
          AND (@EndDate IS NULL OR EntryDate <= @EndDate) 
          AND (@SearchTerm IS NULL OR MedicineName LIKE '%' + @SearchTerm + '%' OR Diagnosis LIKE '%' + @SearchTerm + '%')";

                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new
                {
                    request.InstituteID,
                    StartDate = startDate,
                    EndDate = endDate,
                    SearchTerm = request.SearchTerm
                });

                // SQL query to fetch the stock entries based on the filters
                string sql = @"
        SELECT 
            se.StockID,
            se.ItemTypeID,
            it.ItemType AS ItemTypeName,
            se.MedicineName,
            se.Company,
            se.BatchCode,
            se.Diagnosis,
            se.Quantity,
            se.PricePerQuantity,
            se.ExpiryDate,
            se.EntryDate,
            se.DosageDetails,
            se.InstituteID,
            se.IsActive
        FROM 
            tblStockEntry se
        INNER JOIN 
            tblInfirmaryItemType it ON se.ItemTypeID = it.ItemTypeID
        WHERE
            se.IsActive = 1 
            AND se.InstituteID = @InstituteID
            AND (@StartDate IS NULL OR se.EntryDate >= @StartDate) 
            AND (@EndDate IS NULL OR se.EntryDate <= @EndDate)
            AND (@SearchTerm IS NULL OR se.MedicineName LIKE '%' + @SearchTerm + '%' OR se.Diagnosis LIKE '%' + @SearchTerm + '%')
        ORDER BY 
            se.StockID
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var result = await _connection.QueryAsync<StockEntryResponse>(sql, new
                {
                    request.InstituteID,
                    StartDate = startDate,
                    EndDate = endDate,
                    SearchTerm = request.SearchTerm,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                // Format the ExpiryDate and EntryDate to 'DD-MM-YYYY'
                foreach (var entry in result)
                {
                    if (DateTime.TryParse(entry.ExpiryDate, out DateTime expiryDate))
                    {
                        entry.ExpiryDate = expiryDate.ToString("dd-MM-yyyy");
                    }
                    if (DateTime.TryParse(entry.EntryDate, out DateTime entryDate))
                    {
                        entry.EntryDate = entryDate.ToString("dd-MM-yyyy");
                    }
                }

                return new ServiceResponse<List<StockEntryResponse>>(true, "Records found", result.ToList(), 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StockEntryResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<StockEntry>> GetStockEntryById(int id)
        {
            try
            {
                string query = @"
                SELECT 
                    se.StockID,
                    se.ItemTypeID,
                    it.ItemType AS ItemTypeName,
                    se.MedicineName,
                    se.Company,
                    se.BatchCode,
                    se.Diagnosis,
                    se.Quantity,
                    se.PricePerQuantity,
                    se.ExpiryDate,
                    se.EntryDate,
                    se.DosageDetails,
                    se.InstituteID,
                    se.IsActive
                FROM 
                    tblStockEntry se
                INNER JOIN 
                    tblInfirmaryItemType it ON se.ItemTypeID = it.ItemTypeID
                WHERE 
                    se.StockID = @Id AND se.IsActive = 1";

                var result = await _connection.QueryFirstOrDefaultAsync<StockEntry>(query, new { Id = id });

                // Format the ExpiryDate and EntryDate to 'DD-MM-YYYY'
                if (result != null)
                {
                    if (DateTime.TryParse(result.ExpiryDate, out DateTime expiryDate))
                    {
                        result.ExpiryDate = expiryDate.ToString("dd-MM-yyyy");
                    }

                    if (DateTime.TryParse(result.EntryDate, out DateTime entryDate))
                    {
                        result.EntryDate = entryDate.ToString("dd-MM-yyyy");
                    }

                    return new ServiceResponse<StockEntry>(true, "Record found", result, 200);
                }
                else
                {
                    return new ServiceResponse<StockEntry>(false, "Record not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StockEntry>(false, ex.Message, null, 500);
            }
        }



        public async Task<ServiceResponse<bool>> DeleteStockEntry(int id)
        {
            try
            {
                string query = "UPDATE tblStockEntry SET IsActive = 0 WHERE StockID = @Id";
                int result = await _connection.ExecuteAsync(query, new { Id = id });

                if (result > 0)
                    return new ServiceResponse<bool>(true, "Stock entry deleted successfully", true, 200);
                else
                    return new ServiceResponse<bool>(false, "Operation failed", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<List<GetStockEntriesExportResponse>> GetStockEntriesData(int instituteId, string startDate, string endDate, string searchTerm)
        {
            DateTime? parsedStartDate = null;
            DateTime? parsedEndDate = null;

            // Parse the string dates to DateTime? for SQL compatibility
            if (!string.IsNullOrEmpty(startDate))
            {
                if (DateTime.TryParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStart))
                {
                    parsedStartDate = parsedStart;
                }
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                if (DateTime.TryParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedEnd))
                {
                    parsedEndDate = parsedEnd;
                }
            }

            string query = @"
        SELECT 
            se.StockID,
            it.ItemType AS ItemTypeName,
            se.MedicineName,
            se.Company,
            se.BatchCode,
            se.Diagnosis,
            se.Quantity,
            se.PricePerQuantity,
            se.ExpiryDate,
            se.EntryDate,
            se.DosageDetails
        FROM 
            tblStockEntry se
        INNER JOIN 
            tblInfirmaryItemType it ON se.ItemTypeID = it.ItemTypeID
        WHERE 
            se.IsActive = 1 
            AND se.InstituteID = @InstituteID
            AND (@StartDate IS NULL OR se.EntryDate >= @StartDate)
            AND (@EndDate IS NULL OR se.EntryDate <= @EndDate)
            AND (@SearchTerm IS NULL OR se.MedicineName LIKE '%' + @SearchTerm + '%' OR se.Diagnosis LIKE '%' + @SearchTerm + '%')
        ORDER BY 
            se.StockID";

            return (await _connection.QueryAsync<GetStockEntriesExportResponse>(query, new { InstituteID = instituteId, StartDate = parsedStartDate, EndDate = parsedEndDate, SearchTerm = searchTerm })).AsList();
        }
    }
}
