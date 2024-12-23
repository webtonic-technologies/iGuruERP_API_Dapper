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
    public class InfirmaryRepository : IInfirmaryRepository
    {
        private readonly IDbConnection _connection;

        public InfirmaryRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddUpdateInfirmary(AddUpdateInfirmaryRequest request)
        {
            try
            {
                string query = request.InfirmaryID == 0
                    ? @"INSERT INTO tblInfirmary (InfirmaryName, InfirmaryIncharge, NoOfBeds, Description, InstituteID, IsActive) 
                       VALUES (@InfirmaryName, @InfirmaryIncharge, @NoOfBeds, @Description, @InstituteID, @IsActive)"
                    : @"UPDATE tblInfirmary SET InfirmaryName = @InfirmaryName, InfirmaryIncharge = @InfirmaryIncharge, 
                       NoOfBeds = @NoOfBeds, Description = @Description, InstituteID = @InstituteID, IsActive = @IsActive 
                       WHERE InfirmaryID = @InfirmaryID";

                int result = await _connection.ExecuteAsync(query, request);

                if (result > 0)
                    return new ServiceResponse<string>(true, "Operation Successful", "Infirmary data updated successfully", 200);
                else
                    return new ServiceResponse<string>(false, "Operation Failed", null, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<InfirmaryResponse>>> GetAllInfirmary(GetAllInfirmaryRequest request)
        {
            try
            {
                string countSql = @"SELECT COUNT(*) FROM tblInfirmary WHERE IsActive = 1 AND InstituteID = @InstituteID";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"
                SELECT 
                    i.InfirmaryID,
                    i.InfirmaryName,
                    e.First_Name + ' ' + e.Last_Name AS InfirmaryIncharge, 
                    i.NoOfBeds,
                    i.Description,
                    i.InstituteID,
                    i.IsActive
                FROM 
                    tblInfirmary i
                JOIN 
                    tbl_EmployeeProfileMaster e ON i.InfirmaryIncharge = e.Employee_id
                WHERE
                    i.IsActive = 1 AND i.InstituteID = @InstituteID
                ORDER BY 
                    i.InfirmaryID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var result = await _connection.QueryAsync<InfirmaryResponse>(sql, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new ServiceResponse<List<InfirmaryResponse>>(true, "Records found", result.ToList(), 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<InfirmaryResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Infirmary>> GetInfirmaryById(int id)
        {
            try
            {
                string query = "SELECT * FROM tblInfirmary WHERE InfirmaryID = @Id AND IsActive = 1";
                var result = await _connection.QueryFirstOrDefaultAsync<Infirmary>(query, new { Id = id });

                if (result != null)
                    return new ServiceResponse<Infirmary>(true, "Record found", result, 200);
                else
                    return new ServiceResponse<Infirmary>(false, "Record not found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Infirmary>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteInfirmary(int id)
        {
            try
            {
                string query = "UPDATE tblInfirmary SET IsActive = 0 WHERE InfirmaryID = @Id";
                int result = await _connection.ExecuteAsync(query, new { Id = id });

                if (result > 0)
                    return new ServiceResponse<bool>(true, "Infirmary deleted successfully", true, 200);
                else
                    return new ServiceResponse<bool>(false, "Operation failed", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<List<GetInfirmaryExportResponse>> GetInfirmaryData(int instituteId)
        {
            string query = @"
                SELECT 
                    i.InfirmaryName,
                    e.First_Name + ' ' + e.Last_Name AS InfirmaryIncharge, 
                    i.NoOfBeds,
                    i.Description
                FROM 
                    tblInfirmary i
                JOIN 
                    tbl_EmployeeProfileMaster e ON i.InfirmaryIncharge = e.Employee_id
                WHERE
                    i.IsActive = 1 AND i.InstituteID = @InstituteID
                ORDER BY 
                    i.InfirmaryID";

            return (await _connection.QueryAsync<GetInfirmaryExportResponse>(query, new { InstituteID = instituteId })).AsList();
        }
    }
}
