using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Implementations
{
    public class ItemTypeFetchRepository : IItemTypeFetchRepository
    {
        private readonly IDbConnection _connection;

        public ItemTypeFetchRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<ItemTypeFetchResponse>>> GetAllItemTypesFetch(GetAllItemTypesFetchRequest request)
        {
            try
            {
                string sql = @"
                SELECT 
                    ItemTypeID,
                    ItemType,
                    Description,
                    IsActive
                FROM 
                    tblInfirmaryItemType
                WHERE
                    InstituteID = @InstituteID AND IsActive = 1";

                var result = await _connection.QueryAsync<ItemTypeFetchResponse>(sql, new { request.InstituteID });

                if (result.Any())
                {
                    return new ServiceResponse<List<ItemTypeFetchResponse>>(true, "Records found", result.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<ItemTypeFetchResponse>>(false, "No records found", null, 404);
                }
            }
            catch (System.Exception ex)
            {
                return new ServiceResponse<List<ItemTypeFetchResponse>>(false, ex.Message, null, 500);
            }
        }
    }
}
