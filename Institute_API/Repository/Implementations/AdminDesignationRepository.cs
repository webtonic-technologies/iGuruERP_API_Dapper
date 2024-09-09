using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;
using Dapper;
using Institute_API.DTOs;
using OfficeOpenXml;
using System.Data.Common;

namespace Institute_API.Repository.Implementations
{
    public class AdminDesignationRepository : IAdminDesignationRepository
    {
        private readonly IDbConnection _connection;

        public AdminDesignationRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<string>> AddUpdateAdminDesignation(AdminDesignation request)
        {
            try
            {
                if (request.Designation_id == 0)
                {
                    string sql = @"INSERT INTO [dbo].[tbl_Designation] (Institute_id, DesignationName, Department_id, IsDeleted)
                       VALUES (@Institute_id, @DesignationName, @Department_id, @IsDeleted);
                       SELECT SCOPE_IDENTITY();"; // Retrieve the inserted id
                    request.IsDeleted = false;
                    // Execute the query and retrieve the inserted id
                    int insertedId = await _connection.ExecuteScalarAsync<int>(sql, request);
                    if (insertedId > 0)
                    {
                        string insertQuery = @"INSERT INTO [dbo].[tbl_AdminConfiguration] (Institute_id, Department_id, Designation_id)
                       VALUES (@Institute_id, @Department_id, @Designation_id);";
                        AdminConfiguration adminConfiguration = new()
                        {
                            Institute_id = request.Institute_id,
                            Department_id = request.Department_id,
                            Designation_id = insertedId
                        };
                        // Execute the query and retrieve the inserted id
                        int rowsAffected = await _connection.ExecuteAsync(insertQuery, adminConfiguration);
                        if (rowsAffected > 0)
                        {
                            return new ServiceResponse<string>(true, "Operation successful", "Designation Added successfully", 200);
                        }
                        else
                        {
                            return new ServiceResponse<string>(false, "operation failed", string.Empty, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                    }
                }
                else
                {
                    string sql = @"UPDATE [dbo].[tbl_Designation]
                       SET Institute_id = @Institute_id,
                           DesignationName = @DesignationName,
                           Department_id = @Department_id,
                           IsDeleted = @IsDeleted,
                       WHERE Designation_id = @Designation_id";
                    request.IsDeleted = false;
                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, request);
                    if (affectedRows > 0)
                    {
                        var sqlSelectById = @"
                        SELECT Admin_Configuration_id, Institute_id, Department_id, Designation_id
                        FROM tbl_AdminConfiguration
                        WHERE Designation_id = @Designation_id;";

                        var parameters = new
                        {
                            request.Designation_id
                        };

                        var adminConfiguration = await _connection.QuerySingleOrDefaultAsync<AdminConfiguration>(sqlSelectById, parameters);

                        var sqlUpdate = @"
                        UPDATE tbl_AdminConfiguration
                        SET Institute_id = @Institute_id,
                            Department_id = @Department_id,
                            Designation_id = @Designation_id
                        WHERE Admin_Configuration_id = @Admin_Configuration_id;";
                        AdminConfiguration adminConfiguration1 = new()
                        {
                            Institute_id = request.Institute_id,
                            Department_id = request.Department_id,
                            Designation_id = request.Department_id,
                            Admin_Configuration_id = adminConfiguration != null ? adminConfiguration.Admin_Configuration_id : 0
                        };
                        // Execute the query and retrieve the inserted id
                        int rowsAffected = await _connection.ExecuteAsync(sqlUpdate, adminConfiguration1);
                        if (rowsAffected > 0)
                        {
                            return new ServiceResponse<string>(true, "operation successful", "Designation updated successfully", 200);
                        }
                        else
                        {
                            return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteAdminDesignation(int designationId)
        {
            try
            {
                // Check if the designation is mapped to any employees
                string checkMappingSql = @"SELECT COUNT(*) FROM [tbl_EmployeeProfileMaster] WHERE Designation_id = @DesignationId AND Status = 1"; // Assuming Status = 1 means active
                int employeeCount = await _connection.ExecuteScalarAsync<int>(checkMappingSql, new { DesignationId = designationId });

                if (employeeCount > 0)
                {
                    return new ServiceResponse<string>(false, "Cannot delete the designation; it is mapped to active employees.", string.Empty, 400);
                }

                // Proceed with the soft delete
                string sql = "UPDATE tbl_Designation SET IsDeleted = @IsDeleted WHERE Designation_id = @DesignationId";

                int affectedRows = await _connection.ExecuteAsync(sql, new { IsDeleted = true, DesignationId = designationId });
                if (affectedRows > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successful", "Designation deleted successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation failed", string.Empty, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<AdminDesignationResponse>> GetAdminDesignationById(int Designationid)
        {
            try
            {
                string sql = @"
            SELECT d.Department_id, d.Institute_id, d.DepartmentName as Department_name, 
                   des.Designation_id, des.DesignationName, des.IsDeleted 
            FROM [dbo].[tbl_Designation] des
            JOIN [dbo].[tbl_Department] d ON des.Department_id = d.Department_id
            WHERE des.Designation_id = @Designation_id AND des.IsDeleted = 0";

                // Execute the query and retrieve the designation along with department details
                var designation = await _connection.QueryFirstOrDefaultAsync<AdminDesignationResponse>(sql, new { Designation_id = Designationid });

                if (designation != null)
                {
                    return new ServiceResponse<AdminDesignationResponse>(true, "Record found", designation, 200);
                }
                else
                {
                    return new ServiceResponse<AdminDesignationResponse>(false, "Record not found", new AdminDesignationResponse(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AdminDesignationResponse>(false, ex.Message, new AdminDesignationResponse(), 500);
            }
        }

        public async Task<ServiceResponse<List<AdminDesignationResponse>>> GetAdminDesignationList(GetListRequest request)
        {
            try
            {
                string sql = @"
            SELECT d.Department_id, d.Institute_id, d.DepartmentName as Department_name, 
                   des.Designation_id, des.DesignationName, des.IsDeleted 
            FROM [dbo].[tbl_Designation] des
            JOIN [dbo].[tbl_Department] d ON des.Department_id = d.Department_id
            WHERE des.Institute_id = @Institute_id AND des.IsDeleted = 0";

                // Add search text filter if provided
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    sql += " AND (des.DesignationName LIKE @SearchText OR d.DepartmentName LIKE @SearchText)";
                }

                // Add sorting
                string sortColumn = "des.DesignationName"; // Default sort column
                string sortOrder = request.SortDirection.ToUpper() == "DESC" ? "DESC" : "ASC"; // Validate sort direction

                sql += $" ORDER BY {sortColumn} {sortOrder}";

                // Execute the query and retrieve the designations
                var designations = await _connection.QueryAsync<AdminDesignationResponse>(sql, new
                {
                    Institute_id = request.Institute_id,
                    SearchText = "%" + request.SearchText + "%"
                });

                if (designations != null && designations.Any())
                {
                    // Apply pagination
                    var paginatedDesignations = designations
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToList();

                    return new ServiceResponse<List<AdminDesignationResponse>>(true, "Records found", paginatedDesignations, 200, designations.Count());
                }
                else
                {
                    return new ServiceResponse<List<AdminDesignationResponse>>(false, "Records not found", new List<AdminDesignationResponse>(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AdminDesignationResponse>>(false, ex.Message, new List<AdminDesignationResponse>(), 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // SQL query to fetch Department Name and Designation Name for the given InstituteId
                string sql = @"
            SELECT d.DepartmentName, des.DesignationName
            FROM tbl_Designation des
            JOIN tbl_Department d ON des.Department_id = d.Department_id
            WHERE des.Institute_id = @InstituteId AND des.IsDeleted = 0 AND d.IsDeleted = 0";

                // Execute the query and fetch the designation and department data
                var designationData = await _connection.QueryAsync<dynamic>(sql, new { InstituteId });

                // Check if any data is returned
                if (designationData == null || !designationData.Any())
                {
                    return new ServiceResponse<byte[]>(false, "No data found for the given Institute ID", null, 404);
                }

                // Create a new Excel package
                using (var package = new ExcelPackage())
                {
                    // Add a new worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Designations");

                    // Add headers to the Excel sheet
                    worksheet.Cells[1, 1].Value = "Sr No.";
                    worksheet.Cells[1, 2].Value = "Department Name";
                    worksheet.Cells[1, 3].Value = "Designation Name";

                    // Add the department and designation data to the Excel sheet
                    int row = 2;
                    int srNo = 1;
                    foreach (var record in designationData)
                    {
                        worksheet.Cells[row, 1].Value = srNo;  // Sr No.
                        worksheet.Cells[row, 2].Value = record.DepartmentName;  // Department Name
                        worksheet.Cells[row, 3].Value = record.DesignationName;  // Designation Name
                        row++;
                        srNo++;
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Save the Excel package to a memory stream
                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        var fileBytes = stream.ToArray();

                        // Return the Excel file as a byte array
                        return new ServiceResponse<byte[]>(true, "Excel sheet generated successfully", fileBytes, 200);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, null, 500);
            }
        }
    }
}
