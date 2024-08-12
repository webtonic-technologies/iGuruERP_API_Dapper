using Dapper;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Implementations
{
    public class InfirmaryVisitorTypeRepository : IInfirmaryVisitorTypeRepository
    {
        private readonly IDbConnection _connection;

        public InfirmaryVisitorTypeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<InfirmaryVisitorTypeResponse>>> GetAllInfirmaryVisitorTypes()
        {
            try
            {
                string sql = @"
                SELECT 
                    VisitorTypeID,
                    VisitorType
                FROM 
                    tblInfirmaryVisitorType";

                var result = await _connection.QueryAsync<InfirmaryVisitorTypeResponse>(sql);

                if (result.Any())
                {
                    return new ServiceResponse<List<InfirmaryVisitorTypeResponse>>(true, "Records found", result.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<InfirmaryVisitorTypeResponse>>(false, "No records found", null, 404);
                }
            }
            catch (System.Exception ex)
            {
                return new ServiceResponse<List<InfirmaryVisitorTypeResponse>>(false, ex.Message, null, 500);
            }
        }
    }
}
