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
                string query = request.ItemTypeID == 0
                    ? @"INSERT INTO tblInfirmaryItemType (ItemType, Description, IsActive, InstituteID) 
                       VALUES (@ItemTypeName, @Description, @IsActive, @InstituteID)"
                    : @"UPDATE tblInfirmaryItemType SET ItemType = @ItemTypeName, Description = @Description, 
                       IsActive = @IsActive, InstituteID = @InstituteID 
                       WHERE ItemTypeID = @ItemTypeID";

                int result = await _connection.ExecuteAsync(query, request);

                if (result > 0)
                    return new ServiceResponse<string>(true, "Operation Successful", "ItemType data updated successfully", 200);
                else
                    return new ServiceResponse<string>(false, "Operation Failed", null, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
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
                    ItemType,
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
                string query = "SELECT * FROM tblInfirmaryItemType WHERE ItemTypeID = @Id AND IsActive = 1";
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
