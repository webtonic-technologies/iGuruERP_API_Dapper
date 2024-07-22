using Dapper;
using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LibraryManagement_API.Repository.Implementations
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly IDbConnection _connection;

        public LibraryRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateLibrary(Library request)
        {
            try
            {
                string sql = request.LibraryID == 0 ?
                    @"INSERT INTO tblLibrary (InstituteID, LibraryName, ShortName, LibraryInchargeID, IsActive) 
                      VALUES (@InstituteID, @LibraryName, @ShortName, @LibraryInchargeID, @IsActive)" :
                    @"UPDATE tblLibrary SET InstituteID = @InstituteID, LibraryName = @LibraryName, ShortName = @ShortName, 
                      LibraryInchargeID = @LibraryInchargeID, IsActive = @IsActive WHERE LibraryID = @LibraryID";

                int rowsAffected = await _connection.ExecuteAsync(sql, request);

                return new ServiceResponse<string>(rowsAffected > 0, rowsAffected > 0 ? "Success" : "Failure",
                    rowsAffected > 0 ? "Library saved successfully" : "Failed to save library", rowsAffected > 0 ? 200 : 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<LibraryResponse>>> GetAllLibraries(GetAllLibraryRequest request)
        {
            try
            {
                string countSql = "SELECT COUNT(*) FROM tblLibrary WHERE InstituteID = @InstituteID AND IsActive = 1";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"
                    SELECT 
                        l.LibraryName, 
                        l.ShortName, 
                        CONCAT(e.First_Name, ' ', e.Middle_Name, ' ', e.Last_Name) AS LibraryIncharge 
                    FROM 
                        tblLibrary l
                    LEFT JOIN 
                        tbl_EmployeeProfileMaster e ON l.LibraryInchargeID = e.Employee_id
                    WHERE 
                        l.InstituteID = @InstituteID AND l.IsActive = 1";

                var libraries = await _connection.QueryAsync<LibraryResponse>(sql, new { request.InstituteID });

                var paginatedList = libraries.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                return new ServiceResponse<List<LibraryResponse>>(true, "Records Found", paginatedList, 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<LibraryResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Library>> GetLibraryById(int libraryId)
        {
            try
            {
                string sql = "SELECT * FROM tblLibrary WHERE LibraryID = @LibraryID AND IsActive = 1";
                var library = await _connection.QueryFirstOrDefaultAsync<Library>(sql, new { LibraryID = libraryId });

                return library != null ?
                    new ServiceResponse<Library>(true, "Record Found", library, 200) :
                    new ServiceResponse<Library>(false, "Record Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Library>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteLibrary(int libraryId)
        {
            try
            {
                string sql = "UPDATE tblLibrary SET IsActive = 0 WHERE LibraryID = @LibraryID";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { LibraryID = libraryId });

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
