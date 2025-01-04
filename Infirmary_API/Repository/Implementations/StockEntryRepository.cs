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

                // Query for inserting/updating in tblStockEntry
                string query = request.StockID == 0
                    ? @"INSERT INTO tblStockEntry (ItemTypeID, MedicineName, Company, BatchCode, Diagnosis, Quantity, PricePerQuantity, ExpiryDate, EntryDate, DosageDetails, InstituteID, IsActive, InfirmaryID) 
               VALUES (@ItemTypeID, @MedicineName, @Company, @BatchCode, @Diagnosis, @Quantity, @PricePerQuantity, @ExpiryDate, @EntryDate, @DosageDetails, @InstituteID, @IsActive, @InfirmaryID);
               SELECT CAST(SCOPE_IDENTITY() AS INT);"  // Get the newly inserted StockID
                    : @"UPDATE tblStockEntry SET ItemTypeID = @ItemTypeID, MedicineName = @MedicineName, Company = @Company, 
               BatchCode = @BatchCode, Diagnosis = @Diagnosis, Quantity = @Quantity, PricePerQuantity = @PricePerQuantity, 
               ExpiryDate = @ExpiryDate, EntryDate = @EntryDate, DosageDetails = @DosageDetails, InstituteID = @InstituteID, IsActive = @IsActive, InfirmaryID = @InfirmaryID
               WHERE StockID = @StockID";

                // Execute query for stock entry and get the StockID (if inserting)
                int stockID = await _connection.ExecuteScalarAsync<int>(query, new
                {
                    request.ItemTypeID,
                    request.MedicineName,
                    request.Company,
                    request.BatchCode,
                    request.Diagnosis,
                    request.Quantity,
                    request.PricePerQuantity,
                    ExpiryDate = expiryDate,
                    EntryDate = entryDate,
                    request.DosageDetails,
                    request.InstituteID,
                    request.IsActive,
                    request.StockID,
                    request.InfirmaryID  // Pass InfirmaryID as part of the parameters
                });

                // If the stock entry operation was successful, insert into tblStockQuantity
                if (stockID > 0)
                {
                    string stockQuantityQuery = @"
               INSERT INTO tblStockQuantity (StockID, Quantity, BatchCode, Reason, EntryDate, StockManagerID) 
            VALUES (@StockID, @Quantity, @BatchCode, @Reason, @EntryDate, @StockManagerID)";

                    // If it's a new stock entry, use the returned stockID for tblStockQuantity
                    await _connection.ExecuteAsync(stockQuantityQuery, new
                    {
                        StockID = stockID,  // Use the generated StockID for tblStockQuantity
                        request.Quantity,
                        request.BatchCode,
                        request.Reason,
                        EntryDate = entryDate,
                        StockManagerID = request.StockManagerID
                    });

                    return new ServiceResponse<string>(true, "Operation Successful", "Stock entry updated and quantity added successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation Failed", "Stock entry update failed", 400);
                }
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

        public async Task<bool> EnterStockAdjustment(EnterInfirmaryStockAdjustmentRequest request)
        {
            try
            {
                string query = @"
                    INSERT INTO tblStockQuantity (StockID, Quantity, BatchCode, Reason, EntryDate, StockManagerID)
                    VALUES (@StockID, @Quantity, @BatchCode, @Reason, @EntryDate, @StockManagerID)";

                int result = await _connection.ExecuteAsync(query, new
                {
                    request.StockID,
                    request.Quantity,
                    request.BatchCode,
                    request.Reason,
                    EntryDate = DateTime.ParseExact(request.EntryDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    request.StockManagerID
                });

                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ServiceResponse<List<StockHistoryResponse>>> GetStockHistory(StockHistoryRequest request)
        {
            try
            {
                // Query to fetch stock history data
                string query = @"
                SELECT 
                    'SA#' + CAST(qty.QtyID AS VARCHAR) AS QtyID,
                    qty.BatchCode,
                    qty.Quantity AS QuantityAdjusted,
                    se.Quantity AS CurrentStock,
                    CONCAT(emp.First_Name, ' ', emp.Last_Name, ' on ', CONVERT(VARCHAR, GETDATE(), 105), ' at ', CONVERT(VARCHAR, GETDATE(), 108)) AS UpdatedBy,
                    qty.Reason
                FROM tblStockQuantity qty
                INNER JOIN tblStockEntry se ON se.StockID = qty.StockID
                LEFT JOIN tbl_EmployeeProfileMaster emp ON emp.Employee_id = qty.StockManagerID
                WHERE qty.StockID = @StockID AND se.InstituteID = @InstituteID
                ORDER BY qty.EntryDate DESC";

                // Execute query to fetch stock history
                var result = await _connection.QueryAsync<StockHistoryResponse>(query, new
                {
                    request.StockID,
                    request.InstituteID
                });

                // Query to get the total count of records matching the criteria
                string countQuery = @"
                SELECT COUNT(*) 
                FROM tblStockQuantity qty
                INNER JOIN tblStockEntry se ON se.StockID = qty.StockID
                WHERE qty.StockID = @StockID AND se.InstituteID = @InstituteID";

                // Execute the count query
                int totalCount = await _connection.ExecuteScalarAsync<int>(countQuery, new
                {
                    request.StockID,
                    request.InstituteID
                });

                // Return the stock history along with the total count
                return new ServiceResponse<List<StockHistoryResponse>>(true, "Stock history retrieved successfully", result.ToList(), 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StockHistoryResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<GetStockInfoResponse>> GetStockInfo(GetStockInfoRequest request)
        {
            try
            {
                string query = @"
                    SELECT 
                        inf.InfirmaryName,
                        se.Company AS CompanyName,
                        se.MedicineName AS ItemName,
                        se.PricePerQuantity AS Price,
                        se.Diagnosis
                    FROM tblStockEntry se
                    INNER JOIN tblInfirmary inf ON se.InfirmaryID = inf.InfirmaryID
                    WHERE se.StockID = @StockID AND se.InstituteID = @InstituteID";

                var result = await _connection.QueryFirstOrDefaultAsync<GetStockInfoResponse>(query, new
                {
                    request.StockID,
                    request.InstituteID
                });

                if (result == null)
                {
                    return new ServiceResponse<GetStockInfoResponse>(false, "Stock information not found", null, 404);
                }

                return new ServiceResponse<GetStockInfoResponse>(true, "Stock information retrieved successfully", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetStockInfoResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<List<GetStockHistoryExportResponse>> GetStockHistoryExport(GetStockHistoryExportRequest request)
        {
            string query = @"
                SELECT 
                    'SA#' + CAST(qty.QtyID AS VARCHAR) AS QtyID,
                    qty.BatchCode,
                    qty.Quantity AS QuantityAdjusted,
                    se.Quantity AS CurrentStock,
                    CONCAT(emp.First_Name, ' ', emp.Last_Name, ' on ', CONVERT(VARCHAR, GETDATE(), 105), ' at ', CONVERT(VARCHAR, GETDATE(), 108)) AS UpdatedBy,
                    qty.Reason
                FROM tblStockQuantity qty
                INNER JOIN tblStockEntry se ON se.StockID = qty.StockID
                LEFT JOIN tbl_EmployeeProfileMaster emp ON emp.Employee_id = qty.StockManagerID
                WHERE qty.StockID = @StockID AND se.InstituteID = @InstituteID
                ORDER BY qty.EntryDate DESC";

            var result = await _connection.QueryAsync<GetStockHistoryExportResponse>(query, new
            {
                request.StockID,
                request.InstituteID
            });

            return result.ToList();
        }

    }
}
