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
    public class AuthorRepository : IAuthorRepository
    {
        private readonly IDbConnection _connection;

        public AuthorRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateAuthor(Author request)
        {
            try
            {
                string sql = request.AuthorID == 0 ?
                    @"INSERT INTO tblAuthor (InstituteID, AuthorName, IsActive) 
                      VALUES (@InstituteID, @AuthorName, @IsActive)" :
                    @"UPDATE tblAuthor SET InstituteID = @InstituteID, AuthorName = @AuthorName, 
                      IsActive = @IsActive WHERE AuthorID = @AuthorID";

                int rowsAffected = await _connection.ExecuteAsync(sql, request);

                return new ServiceResponse<string>(rowsAffected > 0, rowsAffected > 0 ? "Success" : "Failure",
                    rowsAffected > 0 ? "Author saved successfully" : "Failed to save author", rowsAffected > 0 ? 200 : 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<AuthorResponse>>> GetAllAuthors(GetAllAuthorsRequest request)
        {
            try
            {
                string countSql = "SELECT COUNT(*) FROM tblAuthor WHERE InstituteID = @InstituteID AND IsActive = 1";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"SELECT AuthorID, InstituteID, AuthorName, IsActive 
                               FROM tblAuthor 
                               WHERE InstituteID = @InstituteID AND IsActive = 1";

                var authors = await _connection.QueryAsync<AuthorResponse>(sql, new { request.InstituteID });

                var paginatedList = authors.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                return new ServiceResponse<List<AuthorResponse>>(true, "Records Found", paginatedList, 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AuthorResponse>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<Author>> GetAuthorById(int authorId)
        {
            try
            {
                string sql = @"SELECT AuthorID, InstituteID, AuthorName, IsActive 
                               FROM tblAuthor 
                               WHERE AuthorID = @AuthorID AND IsActive = 1";
                var author = await _connection.QueryFirstOrDefaultAsync<Author>(sql, new { AuthorID = authorId });

                return author != null ?
                    new ServiceResponse<Author>(true, "Record Found", author, 200) :
                    new ServiceResponse<Author>(false, "Record Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Author>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAuthor(int authorId)
        {
            try
            {
                string sql = "UPDATE tblAuthor SET IsActive = 0 WHERE AuthorID = @AuthorID";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { AuthorID = authorId });

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
