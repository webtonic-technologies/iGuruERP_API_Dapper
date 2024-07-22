using Dapper;
using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryManagement_API.Repository.Implementations
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly IDbConnection _connection;

        public LanguageRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateLanguage(Language request)
        {
            try
            {
                string sql = request.LanguageID == 0 ?
                    @"INSERT INTO tblLibraryLanguage (InstituteID, LanguageName, IsActive) 
                      VALUES (@InstituteID, @LanguageName, @IsActive)" :
                    @"UPDATE tblLibraryLanguage SET InstituteID = @InstituteID, LanguageName = @LanguageName, 
                      IsActive = @IsActive WHERE LanguageID = @LanguageID";

                int rowsAffected = await _connection.ExecuteAsync(sql, request);

                return new ServiceResponse<string>(rowsAffected > 0, rowsAffected > 0 ? "Success" : "Failure",
                    rowsAffected > 0 ? "Language saved successfully" : "Failed to save language", rowsAffected > 0 ? 200 : 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<LanguageResponse>>> GetAllLanguages(GetAllLanguageRequest request)
        {
            try
            {
                string countSql = "SELECT COUNT(*) FROM tblLibraryLanguage WHERE InstituteID = @InstituteID AND IsActive = 1";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"SELECT LanguageID, InstituteID, LanguageName, IsActive 
                               FROM tblLibraryLanguage 
                               WHERE InstituteID = @InstituteID AND IsActive = 1";

                var languages = await _connection.QueryAsync<LanguageResponse>(sql, new { request.InstituteID });

                var paginatedList = languages.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                return new ServiceResponse<List<LanguageResponse>>(true, "Records Found", paginatedList, 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<LanguageResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Language>> GetLanguageById(int languageId)
        {
            try
            {
                string sql = @"SELECT LanguageID, InstituteID, LanguageName, IsActive 
                               FROM tblLibraryLanguage 
                               WHERE LanguageID = @LanguageID AND IsActive = 1";
                var language = await _connection.QueryFirstOrDefaultAsync<Language>(sql, new { LanguageID = languageId });

                return language != null ?
                    new ServiceResponse<Language>(true, "Record Found", language, 200) :
                    new ServiceResponse<Language>(false, "Record Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Language>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteLanguage(int languageId)
        {
            try
            {
                string sql = "UPDATE tblLibraryLanguage SET IsActive = 0 WHERE LanguageID = @LanguageID";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { LanguageID = languageId });

                return new ServiceResponse<bool>(rowsAffected > 0, rowsAffected > 0 ? "Deleted Successfully" : "Delete Failed",
                    rowsAffected > 0, rowsAffected > 0 ? 200 : 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
