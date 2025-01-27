using Dapper;
using LibraryManagement_API.DTOs.Requests; // Ensure this using directive is present
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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnection _connection;

        public CategoryRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateCategories(List<Category> requests)
        {
            using (var connection = _connection)
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (var category in requests)
                        {
                            string sql = category.CategoryID == 0
                                ? @"INSERT INTO tblLibraryCategory (InstituteID, LibraryCategoryName, Code, IsActive) 
                              VALUES (@InstituteID, @LibraryCategoryName, @Code, @IsActive)"
                                : @"UPDATE tblLibraryCategory 
                              SET InstituteID = @InstituteID, 
                                  LibraryCategoryName = @LibraryCategoryName, 
                                  Code = @Code, 
                                  IsActive = @IsActive 
                              WHERE LibraryCategoryID = @CategoryID";

                            await connection.ExecuteAsync(sql, new
                            {
                                category.InstituteID,
                                LibraryCategoryName = category.CategoryName, // Map CategoryName to LibraryCategoryName
                                category.Code,
                                category.IsActive,
                                category.CategoryID
                            }, transaction);
                        }

                        // Commit the transaction if all operations succeed
                        transaction.Commit();
                        return new ServiceResponse<string>(true, "All categories saved successfully", null, 200);
                    }
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of failure
                    return new ServiceResponse<string>(false, ex.Message, null, 500);
                }
            }
        }


        //public async Task<ServiceResponse<string>> AddUpdateCategory(Category request)
        //{
        //    try
        //    {
        //        string sql = request.CategoryID == 0 ?
        //            @"INSERT INTO tblLibraryCategory (InstituteID, LibraryCategoryName, Code, IsActive) 
        //              VALUES (@InstituteID, @LibraryCategoryName, @Code, @IsActive)" :
        //            @"UPDATE tblLibraryCategory SET InstituteID = @InstituteID, LibraryCategoryName = @LibraryCategoryName, 
        //              Code = @Code, IsActive = @IsActive WHERE LibraryCategoryID = @CategoryID";

        //        int rowsAffected = await _connection.ExecuteAsync(sql, new
        //        {
        //            request.InstituteID,
        //            LibraryCategoryName = request.CategoryName, // Map CategoryName to LibraryCategoryName
        //            request.Code,
        //            request.IsActive,
        //            request.CategoryID
        //        });

        //        return new ServiceResponse<string>(rowsAffected > 0, rowsAffected > 0 ? "Success" : "Failure",
        //            rowsAffected > 0 ? "Category saved successfully" : "Failed to save category", rowsAffected > 0 ? 200 : 400);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<string>(false, ex.Message, null, 500);
        //    }
        //}

        public async Task<ServiceResponse<List<CategoryResponse>>> GetAllCategories(GetAllCategoriesRequest request)
        {
            try
            {
                string countSql = "SELECT COUNT(*) FROM tblLibraryCategory WHERE InstituteID = @InstituteID AND IsActive = 1";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"SELECT LibraryCategoryID AS CategoryID, InstituteID, LibraryCategoryName AS CategoryName, Code, IsActive 
                               FROM tblLibraryCategory 
                               WHERE InstituteID = @InstituteID AND IsActive = 1";

                var categories = await _connection.QueryAsync<CategoryResponse>(sql, new { request.InstituteID });

                var paginatedList = categories.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                return new ServiceResponse<List<CategoryResponse>>(true, "Records Found", paginatedList, 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CategoryResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Category>> GetCategoryById(int categoryId)
        {
            try
            {
                string sql = @"SELECT LibraryCategoryID AS CategoryID, InstituteID, LibraryCategoryName AS CategoryName, Code, IsActive 
                               FROM tblLibraryCategory 
                               WHERE LibraryCategoryID = @LibraryCategoryID AND IsActive = 1";
                var category = await _connection.QueryFirstOrDefaultAsync<Category>(sql, new { LibraryCategoryID = categoryId });

                return category != null ?
                    new ServiceResponse<Category>(true, "Record Found", category, 200) :
                    new ServiceResponse<Category>(false, "Record Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Category>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCategory(int categoryId)
        {
            try
            {
                string sql = "UPDATE tblLibraryCategory SET IsActive = 0 WHERE LibraryCategoryID = @LibraryCategoryID";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { LibraryCategoryID = categoryId });

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
