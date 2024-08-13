using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Implementations
{
    public class ItemTypeRepository : IItemTypeRepository
    {
        private readonly IDbConnection _connection;

        public ItemTypeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddUpdateItemType(AddUpdateItemTypeRequest request)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var itemType in request.ItemTypes)
                        {
                            // Check if the item type already exists
                            var existingItemType = await _connection.QueryFirstOrDefaultAsync<int>(@"
                        SELECT COUNT(1) 
                        FROM tblInfirmaryItemType 
                        WHERE ItemType = @ItemTypeName AND InstituteID = @InstituteID",
                                new { itemType.ItemTypeName, itemType.InstituteID }, transaction);

                            if (existingItemType > 0)
                            {
                                // If the item type already exists, skip the insertion
                                continue;
                            }

                            if (itemType.ItemTypeID == 0)
                            {
                                await _connection.ExecuteAsync(@"
                            INSERT INTO tblInfirmaryItemType (ItemType, Description, InstituteID, IsActive)
                            VALUES (@ItemTypeName, @Description, @InstituteID, @IsActive);",
                                    itemType, transaction);
                            }
                            else
                            {
                                await _connection.ExecuteAsync(@"
                            UPDATE tblInfirmaryItemType
                            SET ItemType = @ItemTypeName,
                                Description = @Description,
                                InstituteID = @InstituteID,
                                IsActive = @IsActive
                            WHERE ItemTypeID = @ItemTypeID;",
                                    itemType, transaction);
                            }
                        }

                        transaction.Commit();
                        return new ServiceResponse<string>(true, "Operation Successful", "Items added/updated successfully", 200);
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, null, 500);
                    }
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<ServiceResponse<List<ItemTypeResponse>>> GetAllItemTypes(GetAllItemTypesRequest request)
        {
            try
            {
                string countSql = @"SELECT COUNT(*) FROM tblInfirmaryItemType WHERE IsActive = 1 AND InstituteID = @InstituteID";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"
                SELECT 
                    ItemTypeID,
                    ItemType as ItemTypeName,
                    Description,
                    IsActive,
                    InstituteID
                FROM 
                    tblInfirmaryItemType
                WHERE
                    IsActive = 1 AND InstituteID = @InstituteID
                ORDER BY 
                    ItemTypeID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var result = await _connection.QueryAsync<ItemTypeResponse>(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new ServiceResponse<List<ItemTypeResponse>>(true, "Records found", result.ToList(), 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ItemTypeResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<ItemType>> GetItemTypeById(int id)
        {
            try
            {
                string query = "SELECT ItemTypeID, ItemType as ItemTypeName, Description, IsActive, InstituteID FROM tblInfirmaryItemType WHERE ItemTypeID = @Id AND IsActive = 1";
                var result = await _connection.QueryFirstOrDefaultAsync<ItemType>(query, new { Id = id });

                if (result != null)
                    return new ServiceResponse<ItemType>(true, "Record found", result, 200);
                else
                    return new ServiceResponse<ItemType>(false, "Record not found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ItemType>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteItemType(int id)
        {
            try
            {
                string query = "UPDATE tblInfirmaryItemType SET IsActive = 0 WHERE ItemTypeID = @Id";
                int result = await _connection.ExecuteAsync(query, new { Id = id });

                if (result > 0)
                    return new ServiceResponse<bool>(true, "ItemType deleted successfully", true, 200);
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
