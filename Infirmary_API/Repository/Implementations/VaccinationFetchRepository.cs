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
    public class VaccinationFetchRepository : IVaccinationFetchRepository
    {
        private readonly IDbConnection _connection;

        public VaccinationFetchRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<VaccinationFetchResponse>>> GetAllVaccinationsFetch(GetAllVaccinationsFetchRequest request)
        {
            try
            {
                string sql = @"
                SELECT 
                    VaccinationID,
                    VaccinationName,
                    Description,
                    IsActive
                FROM 
                    tblVaccination
                WHERE
                    InstituteID = @InstituteID AND IsActive = 1";

                var result = await _connection.QueryAsync<VaccinationFetchResponse>(sql, new { request.InstituteID });

                if (result.Any())
                {
                    return new ServiceResponse<List<VaccinationFetchResponse>>(true, "Records found", result.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<VaccinationFetchResponse>>(false, "No records found", null, 404);
                }
            }
            catch (System.Exception ex)
            {
                return new ServiceResponse<List<VaccinationFetchResponse>>(false, ex.Message, null, 500);
            }
        }
    }
}
