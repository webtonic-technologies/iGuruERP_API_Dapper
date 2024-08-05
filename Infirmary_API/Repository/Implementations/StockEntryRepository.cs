using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
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
                string query = request.StockID == 0
                    ? @"INSERT INTO tblStockEntry (ItemTypeID, MedicineName, Company, BatchCode, Diagnosis, Quantity, PricePerQuantity, ExpiryDate, EntryDate, DosageDetails, InstituteID, IsActive) 
                       VALUES (@ItemTypeID, @MedicineName, @Company, @BatchCode, @Diagnosis, @Quantity, @PricePerQuantity, @ExpiryDate, @EntryDate, @DosageDetails, @InstituteID, @IsActive)"
                    : @"UPDATE tblStockEntry SET ItemTypeID = @ItemTypeID, MedicineName = @MedicineName, Company = @Company, 
                       BatchCode = @BatchCode, Diagnosis = @Diagnosis, Quantity = @Quantity, PricePerQuantity = @PricePerQuantity, 
                       ExpiryDate = @ExpiryDate, EntryDate = @EntryDate, DosageDetails = @DosageDetails, InstituteID = @InstituteID, IsActive = @IsActive 
                       WHERE StockID = @StockID";

                int result = await _connection.ExecuteAsync(query, request);

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
                string countSql = @"SELECT COUNT(*) FROM tblStockEntry WHERE IsActive = 1 AND InstituteID = @InstituteID";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"
                SELECT 
                    StockID,
                    ItemTypeID,
                    MedicineName,
                    Company,
                    BatchCode,
                    Diagnosis,
                    Quantity,
                    PricePerQuantity,
                    ExpiryDate,
                    EntryDate,
                    DosageDetails,
                    InstituteID,
                    IsActive
                FROM 
                    tblStockEntry
                WHERE
                    IsActive = 1 AND InstituteID = @InstituteID
                ORDER BY 
                    StockID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var result = await _connection.QueryAsync<StockEntryResponse>(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

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
                string query = "SELECT * FROM tblStockEntry WHERE StockID = @Id AND IsActive = 1";
                var result = await _connection.QueryFirstOrDefaultAsync<StockEntry>(query, new { Id = id });

                if (result != null)
                    return new ServiceResponse<StockEntry>(true, "Record found", result, 200);
                else
                    return new ServiceResponse<StockEntry>(false, "Record not found", null, 404);
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
    }
}
