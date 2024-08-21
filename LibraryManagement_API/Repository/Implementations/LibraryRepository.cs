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

        private IDbConnection CreateConnection()
        {
            return _connection; // Use _connection instead of _configuration
        }

        public async Task<ServiceResponse<string>> AddUpdateLibrary(AddUpdateLibraryRequest request)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var library in request.Libraries)
                        {
                            string sql;

                            if (library.LibraryID == 0)
                            {
                                sql = @"INSERT INTO tblLibrary (InstituteID, LibraryName, ShortName, LibraryInchargeID, IsActive) 
                                        VALUES (@InstituteID, @LibraryName, @ShortName, @LibraryInchargeID, @IsActive)";

                                await connection.ExecuteAsync(sql, new
                                {
                                    library.InstituteID,
                                    library.LibraryName,
                                    library.ShortName,
                                    library.LibraryInchargeID,
                                    library.IsActive
                                }, transaction);
                            }
                            else
                            {
                                sql = @"UPDATE tblLibrary 
                                        SET InstituteID = @InstituteID, LibraryName = @LibraryName, ShortName = @ShortName, 
                                            LibraryInchargeID = @LibraryInchargeID, IsActive = @IsActive
                                        WHERE LibraryID = @LibraryID";

                                await connection.ExecuteAsync(sql, new
                                {
                                    library.LibraryID,
                                    library.InstituteID,
                                    library.LibraryName,
                                    library.ShortName,
                                    library.LibraryInchargeID,
                                    library.IsActive
                                }, transaction);
                            }
                        }

                        transaction.Commit();
                        return new ServiceResponse<string>(true, "Success", "Libraries saved successfully", 200);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, null, 500);
                    }
                }
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

        public async Task<ServiceResponse<List<LibraryInchargeResponse>>> GetAllLibraryIncharge(GetAllLibraryInchargeRequest request)
        {
            try
            {
                string sql = @"
                    SELECT 
                        Employee_id AS EmployeeID,
                        First_Name AS FirstName,
                        Middle_Name AS MiddleName,
                        Last_Name AS LastName,
                        mobile_number AS MobileNumber,
                        EmailID
                    FROM tbl_EmployeeProfileMaster
                    WHERE Institute_id = @InstituteID 
                      AND Department_id = @DepartmentID 
                      AND Designation_id = @DesignationID";

                var incharges = await _connection.QueryAsync<LibraryInchargeResponse>(sql, new
                {
                    request.InstituteID,
                    request.DepartmentID,
                    request.DesignationID
                });

                return new ServiceResponse<List<LibraryInchargeResponse>>(true, "Records Found", incharges.AsList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<LibraryInchargeResponse>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<List<LibraryFetchResponse>>> GetAllLibraryFetch(GetAllLibraryFetchRequest request)
        {
            try
            {
                string sql = @"
                    SELECT 
                        l.LibraryID,
                        l.InstituteID,
                        l.LibraryName,
                        l.ShortName,
                        CONCAT(e.First_Name, ' ', e.Middle_Name, ' ', e.Last_Name) AS LibraryIncharge
                    FROM 
                        tblLibrary l
                    LEFT JOIN 
                        tbl_EmployeeProfileMaster e ON l.LibraryInchargeID = e.Employee_id
                    WHERE 
                        l.InstituteID = @InstituteID AND l.IsActive = 1";

                var libraries = await _connection.QueryAsync<LibraryFetchResponse>(sql, new { request.InstituteID });

                return new ServiceResponse<List<LibraryFetchResponse>>(true, "Records Found", libraries.AsList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<LibraryFetchResponse>>(false, ex.Message, null, 500);
            }
        }
    }
}
