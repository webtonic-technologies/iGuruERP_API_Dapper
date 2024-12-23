using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Implementations
{
    public class VaccinationRepository : IVaccinationRepository
    {
        private readonly IDbConnection _connection;

        public VaccinationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddUpdateVaccination(AddUpdateVaccinationRequest request)
        {
            try
            {
                string query = request.VaccinationID == 0
                    ? @"INSERT INTO tblVaccination (VaccinationName, Description, IsActive, InstituteID) 
                       VALUES (@VaccinationName, @Description, @IsActive, @InstituteID)"
                    : @"UPDATE tblVaccination SET VaccinationName = @VaccinationName, Description = @Description, 
                       IsActive = @IsActive, InstituteID = @InstituteID 
                       WHERE VaccinationID = @VaccinationID";

                int result = await _connection.ExecuteAsync(query, request);

                if (result > 0)
                    return new ServiceResponse<string>(true, "Operation Successful", "Vaccination data updated successfully", 200);
                else
                    return new ServiceResponse<string>(false, "Operation Failed", null, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<VaccinationResponse>>> GetAllVaccinations(GetAllVaccinationsRequest request)
        {
            try
            {
                string countSql = @"SELECT COUNT(*) FROM tblVaccination WHERE IsActive = 1 AND InstituteID = @InstituteID";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"
                SELECT 
                    VaccinationID,
                    VaccinationName,
                    Description,
                    IsActive,
                    InstituteID
                FROM 
                    tblVaccination
                WHERE
                    IsActive = 1 AND InstituteID = @InstituteID
                ORDER BY 
                    VaccinationID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var result = await _connection.QueryAsync<VaccinationResponse>(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new ServiceResponse<List<VaccinationResponse>>(true, "Records found", result.ToList(), 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<VaccinationResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Vaccination>> GetVaccinationById(int id)
        {
            try
            {
                string query = "SELECT * FROM tblVaccination WHERE VaccinationID = @Id AND IsActive = 1";
                var result = await _connection.QueryFirstOrDefaultAsync<Vaccination>(query, new { Id = id });

                if (result != null)
                    return new ServiceResponse<Vaccination>(true, "Record found", result, 200);
                else
                    return new ServiceResponse<Vaccination>(false, "Record not found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Vaccination>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteVaccination(int id)
        {
            try
            {
                string query = "UPDATE tblVaccination SET IsActive = 0 WHERE VaccinationID = @Id";
                int result = await _connection.ExecuteAsync(query, new { Id = id });

                if (result > 0)
                    return new ServiceResponse<bool>(true, "Vaccination deleted successfully", true, 200);
                else
                    return new ServiceResponse<bool>(false, "Operation failed", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<List<GetVaccinationsExportResponse>> GetVaccinationData(int instituteId)
        {
            string query = @"
                SELECT 
                    VaccinationName,
                    Description
                FROM 
                    tblVaccination
                WHERE
                    IsActive = 1 AND InstituteID = @InstituteID
                ORDER BY 
                    VaccinationID";

            return (await _connection.QueryAsync<GetVaccinationsExportResponse>(query, new { InstituteID = instituteId })).AsList();
        }
    }
}
