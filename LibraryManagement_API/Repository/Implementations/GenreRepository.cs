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
    public class GenreRepository : IGenreRepository
    {
        private readonly IDbConnection _connection;

        public GenreRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateGenre(Genre request)
        {
            try
            {
                string sql = request.GenreID == 0 ?
                    @"INSERT INTO tblGenre (InstituteID, GenreName, Description, IsActive) 
                      VALUES (@InstituteID, @GenreName, @Description, @IsActive)" :
                    @"UPDATE tblGenre SET InstituteID = @InstituteID, GenreName = @GenreName, Description = @Description, 
                      IsActive = @IsActive WHERE GenreID = @GenreID";

                int rowsAffected = await _connection.ExecuteAsync(sql, request);

                return new ServiceResponse<string>(rowsAffected > 0, rowsAffected > 0 ? "Success" : "Failure",
                    rowsAffected > 0 ? "Genre saved successfully" : "Failed to save genre", rowsAffected > 0 ? 200 : 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<GenreResponse>>> GetAllGenres(GetAllGenreRequest request)
        {
            try
            {
                string countSql = "SELECT COUNT(*) FROM tblGenre WHERE InstituteID = @InstituteID AND IsActive = 1";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"SELECT GenreID, InstituteID, GenreName, Description, IsActive 
                               FROM tblGenre 
                               WHERE InstituteID = @InstituteID AND IsActive = 1";

                var genres = await _connection.QueryAsync<GenreResponse>(sql, new { request.InstituteID });

                var paginatedList = genres.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                return new ServiceResponse<List<GenreResponse>>(true, "Records Found", paginatedList, 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GenreResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Genre>> GetGenreById(int genreId)
        {
            try
            {
                string sql = @"SELECT GenreID, InstituteID, GenreName, Description, IsActive 
                               FROM tblGenre 
                               WHERE GenreID = @GenreID AND IsActive = 1";
                var genre = await _connection.QueryFirstOrDefaultAsync<Genre>(sql, new { GenreID = genreId });

                return genre != null ?
                    new ServiceResponse<Genre>(true, "Record Found", genre, 200) :
                    new ServiceResponse<Genre>(false, "Record Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Genre>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteGenre(int genreId)
        {
            try
            {
                string sql = "UPDATE tblGenre SET IsActive = 0 WHERE GenreID = @GenreID";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { GenreID = genreId });

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
