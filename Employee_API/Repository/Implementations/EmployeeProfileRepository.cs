using Dapper;
using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Models;
using Employee_API.Repository.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Text;

namespace Employee_API.Repository.Implementations
{
    public class EmployeeProfileRepository : IEmployeeProfileRepository
    {
        private readonly IDbConnection _connection;
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EmployeeProfileRepository(IDbConnection connection, IWebHostEnvironment hostingEnvironment, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _httpContextAccessor = httpContextAccessor;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }


        public async Task<ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>> GetExportHistoryByInstituteId(int instituteId)
        {
            try
            {
                string query = @"SELECT HistoryId, EmployeeCount, DownloadDate, IPAddress, Username, InstituteId 
                         FROM tbl_EmployeeExportHistory 
                         WHERE InstituteId = @InstituteId";

                var historyRecords = await _connection.QueryAsync<EmployeeExportHistoryDto>(query, new { InstituteId = instituteId });

                if (!historyRecords.Any())
                {
                    return new ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>(false, "No records found", null, 204);
                }

                // Format the DownloadDate before returning
                var formattedRecords = historyRecords.Select(record => new EmployeeExportHistoryDto
                {
                    HistoryId = record.HistoryId,
                    EmployeeCount = record.EmployeeCount,
                    // Format the date here
                    DownloadDate = !string.IsNullOrEmpty(record.DownloadDate)
                   ? DateTime.Parse(record.DownloadDate).ToString("dd-MM-yyyy at hh:mm tt")
                   : null, // Set to null if DownloadDate is not available
                    IPAddress = record.IPAddress,
                    Username = record.Username,
                    InstituteId = record.InstituteId
                });

                return new ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>(true, "Records found", formattedRecords, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>> GetBulkHistoryByInstituteId(int instituteId)
        {
            try
            {
                string query = @"SELECT HistoryId, EmployeeCount, DownloadDate, IPAddress, Username, InstituteId 
                         FROM tbl_EmployeeBulkUpdateHistory 
                         WHERE InstituteId = @InstituteId";

                var historyRecords = await _connection.QueryAsync<EmployeeExportHistoryDto>(query, new { InstituteId = instituteId });

                if (!historyRecords.Any())
                {
                    return new ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>(false, "No records found", null, 204);
                }

                // Format the DownloadDate before returning
                var formattedRecords = historyRecords.Select(record => new EmployeeExportHistoryDto
                {
                    HistoryId = record.HistoryId,
                    EmployeeCount = record.EmployeeCount,
                    // Format the date here
                    DownloadDate = !string.IsNullOrEmpty(record.DownloadDate)
                   ? DateTime.Parse(record.DownloadDate).ToString("dd-MM-yyyy at hh:mm tt")
                   : null, // Set to null if DownloadDate is not available
                    IPAddress = record.IPAddress,
                    Username = record.Username,
                    InstituteId = record.InstituteId
                });

                return new ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>(true, "Records found", formattedRecords, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<EmployeeExportHistoryDto>>(false, ex.Message, null, 500);
            }
        }
        public async Task<IEnumerable<dynamic>> ParseExcelFile(IFormFile file, int instituteId)
        {
            var result = new List<ExpandoObject>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                        throw new Exception("No worksheet found");

                    // Get the number of rows and columns in the Excel file
                    var rowCount = worksheet.Dimension.Rows;
                    var columnCount = worksheet.Dimension.Columns;

                    // Get the header row (dynamic column names)
                    var columnHeaders = new List<string>();
                    for (int col = 1; col <= columnCount; col++)
                    {
                        columnHeaders.Add(worksheet.Cells[1, col].Text);
                    }

                    // Iterate over the rows
                    for (int row = 2; row <= rowCount; row++) // Skipping header row
                    {
                        dynamic rowData = new ExpandoObject();
                        var rowDict = (IDictionary<string, object>)rowData;

                        for (int col = 1; col <= columnCount; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Text;
                            var columnHeader = columnHeaders[col - 1];

                            rowDict[columnHeader] = cellValue;
                        }

                        result.Add(rowData);
                    }
                }
            }

            // You can also apply filters based on the instituteId or other parameters
            return result;
        }
        public async Task<ServiceResponse<int>> AddUpdateEmployeeProfile(EmployeeProfile request)
        {
            try
            {
                // Convert Date_of_Joining and Date_of_Birth to IST and format to DD-MM-YYYY
                var dateOfJoining = request.Date_of_Joining?.AddHours(5).AddMinutes(30).ToString("dd-MM-yyyy");
                var dateOfBirth = request.Date_of_Birth?.AddHours(5).AddMinutes(30).ToString("dd-MM-yyyy");

                if (request.Employee_id == 0)
                {
                    string sql = @"INSERT INTO [dbo].[tbl_EmployeeProfileMaster] 
                 (First_Name, Middle_Name, Last_Name, Gender_id, Department_id, Designation_id, mobile_number, 
                  Date_of_Joining, Nationality_id, Religion_id, Date_of_Birth, EmailID, Employee_code_id, marrital_status_id, 
                  Blood_Group_id, aadhar_no, pan_no, EPF_no, ESIC_no, Institute_id, EmpPhoto, uan_no, Status) 
                VALUES 
                 (@First_Name, @Middle_Name, @Last_Name, @Gender_id, @Department_id, @Designation_id, @mobile_number, 
                  @Date_of_Joining, @Nationality_id, @Religion_id, @Date_of_Birth, @EmailID, @Employee_code_id, @marrital_status_id, 
                  @Blood_Group_id, @aadhar_no, @pan_no, @EPF_no, @ESIC_no, @Institute_id, @EmpPhoto, @uan_no, @Status);
                SELECT SCOPE_IDENTITY();"; // Retrieve the inserted Employee_id

                    // Execute the query and retrieve the inserted Employee_id
                    int employeeId = await _connection.ExecuteScalarAsync<int>(sql, new
                    {
                        request.First_Name,
                        request.Middle_Name,
                        request.Last_Name,
                        request.Gender_id,
                        request.Department_id,
                        request.Designation_id,
                        request.mobile_number,
                        Date_of_Joining = dateOfJoining,
                        request.Nationality_id,
                        request.Religion_id,
                        Date_of_Birth = dateOfBirth,
                        request.EmailID,
                        request.Employee_code_id,
                        request.marrital_status_id,
                        request.Blood_Group_id,
                        request.aadhar_no,
                        request.pan_no,
                        request.EPF_no,
                        request.ESIC_no,
                        request.Institute_id,
                        request.uan_no,
                        request.Status,
                        EmpPhoto = ImageUpload(request.EmpPhoto)
                    });

                    if (employeeId > 0)
                    {
                        // Additional logic to handle related entities
                        request.Family.Employee_id = employeeId;
                        var empfam = await AddUpdateEmployeeFamily(request.Family ??= new EmployeeFamily());
                        var empdoc = await AddUpdateEmployeeDocuments(request.EmployeeDocuments ??= [], employeeId);
                        var empQua = await AddUpdateEmployeeQualification(request.EmployeeQualifications ??= [], employeeId);
                        var empwork = await AddUpdateEmployeeWorkExp(request.EmployeeWorkExperiences ??= [], employeeId);
                        var empbank = await AddUpdateEmployeeBankDetails(request.EmployeeBankDetails ??= [], employeeId);
                        var empadd = await AddUpdateEmployeeAddressDetails(request.EmployeeAddressDetails, employeeId);
                        request.EmployeeStaffMappingRequest.EmployeeId = employeeId;
                        var mapp = await AddUpdateEmployeeStaffMapping(request.EmployeeStaffMappingRequest);
                        var userlog = await CreateUserLoginInfo(employeeId, 1, request.Institute_id);
                        return new ServiceResponse<int>(true, "Operation successful", employeeId, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occurred", 0, 500);
                    }
                }
                else
                {
                    string sql = @"UPDATE [dbo].[tbl_EmployeeProfileMaster] SET 
                 First_Name = @First_Name, 
                 Middle_Name = @Middle_Name, 
                 Last_Name = @Last_Name, 
                 Gender_id = @Gender_id, 
                 Department_id = @Department_id, 
                 Designation_id = @Designation_id, 
                 mobile_number = @mobile_number, 
                 Date_of_Joining = @Date_of_Joining, 
                 Nationality_id = @Nationality_id, 
                 Religion_id = @Religion_id, 
                 Date_of_Birth = @Date_of_Birth, 
                 EmailID = @EmailID, 
                 Employee_code_id = @Employee_code_id, 
                 marrital_status_id = @marrital_status_id, 
                 Blood_Group_id = @Blood_Group_id, 
                 aadhar_no = @aadhar_no, 
                 pan_no = @pan_no, 
                 EPF_no = @EPF_no, 
                 ESIC_no = @ESIC_no, 
                 Institute_id = @Institute_id,
                 EmpPhoto = @EmpPhoto,
                 uan_no = @uan_no,
                 Status = @Status
               WHERE Employee_id = @Employee_id";

                    // Execute the query
                     var connection = new SqlConnection(_connectionString);
                    int rowsAffected = await connection.ExecuteAsync(sql, new
                    {
                        request.Employee_id,
                        request.First_Name,
                        request.Middle_Name,
                        request.Last_Name,
                        request.Gender_id,
                        request.Department_id,
                        request.Designation_id,
                        request.mobile_number,
                        Date_of_Joining = dateOfJoining,
                        request.Nationality_id,
                        request.Religion_id,
                        Date_of_Birth = dateOfBirth,
                        request.EmailID,
                        request.Employee_code_id,
                        request.marrital_status_id,
                        request.Blood_Group_id,
                        request.aadhar_no,
                        request.pan_no,
                        request.EPF_no,
                        request.ESIC_no,
                        request.Institute_id,
                        request.uan_no,
                        request.Status,
                        EmpPhoto = ImageUpload(request.EmpPhoto)
                    });

                    if (rowsAffected > 0)
                    {
                        // Additional logic to handle related entities
                        request.Family.Employee_id = request.Employee_id;
                        var empfam = await AddUpdateEmployeeFamily(request.Family ??= new EmployeeFamily());
                        var empdoc = await AddUpdateEmployeeDocuments(request.EmployeeDocuments ??= [], request.Employee_id);
                        var empQua = await AddUpdateEmployeeQualification(request.EmployeeQualifications ??= [], request.Employee_id);
                        var empwork = await AddUpdateEmployeeWorkExp(request.EmployeeWorkExperiences ??= [], request.Employee_id);
                        var empbank = await AddUpdateEmployeeBankDetails(request.EmployeeBankDetails ??= [], request.Employee_id);
                        var empadd = await AddUpdateEmployeeAddressDetails(request.EmployeeAddressDetails ??= [], request.Employee_id);
                        if (request.EmployeeStaffMappingRequest != null)
                        {
                            request.EmployeeStaffMappingRequest.EmployeeId = request.Employee_id;
                            var mapp = await AddUpdateEmployeeStaffMapping(request.EmployeeStaffMappingRequest);
                        }
                        return new ServiceResponse<int>(true, "Operation successful", request.Employee_id, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occurred", 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<string>> BulkUpdateEmployee(List<EmployeeProfile> request)
        {
            try
            {
                int InstituteId = 0;
                // Check if the request is null or empty
                if (request == null || !request.Any())
                {
                    return new ServiceResponse<string>(false, "Empty employee list provided", string.Empty, 400);
                }

                // Iterate through each employee in the list and update their details
                foreach (var employee in request)
                {
                    InstituteId = employee.Institute_id;
                    // Convert Date_of_Joining and Date_of_Birth to IST and format to DD-MM-YYYY
                    var dateOfJoining = employee.Date_of_Joining?.AddHours(5).AddMinutes(30).ToString("dd-MM-yyyy");
                    var dateOfBirth = employee.Date_of_Birth?.AddHours(5).AddMinutes(30).ToString("dd-MM-yyyy");

                    string sql = @"UPDATE [dbo].[tbl_EmployeeProfileMaster] SET 
              First_Name = @First_Name, 
              Middle_Name = @Middle_Name, 
              Last_Name = @Last_Name, 
              Gender_id = @Gender_id, 
              Department_id = @Department_id, 
              Designation_id = @Designation_id, 
              mobile_number = @mobile_number, 
              Date_of_Joining = @Date_of_Joining, 
              Nationality_id = @Nationality_id, 
              Religion_id = @Religion_id, 
              Date_of_Birth = @Date_of_Birth, 
              EmailID = @EmailID, 
              Employee_code_id = @Employee_code_id, 
              marrital_status_id = @marrital_status_id, 
              Blood_Group_id = @Blood_Group_id, 
              aadhar_no = @aadhar_no, 
              pan_no = @pan_no, 
              EPF_no = @EPF_no, 
              ESIC_no = @ESIC_no, 
              Institute_id = @Institute_id,
              EmpPhoto = @EmpPhoto,
              uan_no = @uan_no,
              Status = @Status
            WHERE Employee_id = @Employee_id";

                    // Execute the update query for the current employee
                    int rowsAffected = await _connection.ExecuteAsync(sql, new
                    {
                        employee.Employee_id,
                        employee.First_Name,
                        employee.Middle_Name,
                        employee.Last_Name,
                        employee.Gender_id,
                        employee.Department_id,
                        employee.Designation_id,
                        employee.mobile_number,
                        Date_of_Joining = dateOfJoining,
                        employee.Nationality_id,
                        employee.Religion_id,
                        Date_of_Birth = dateOfBirth,
                        employee.EmailID,
                        employee.Employee_code_id,
                        employee.marrital_status_id,
                        employee.Blood_Group_id,
                        employee.aadhar_no,
                        employee.pan_no,
                        employee.EPF_no,
                        employee.ESIC_no,
                        employee.Institute_id,
                        employee.uan_no,
                        employee.Status,
                        EmpPhoto = ImageUpload(employee.EmpPhoto) // Handle image upload if necessary
                    });
                    var ipAddress = GetClientIPAddress();
                    string historyQuery = @"insert into tbl_EmployeeBulkUpdateHistory ( EmployeeCount ,DownloadDate ,IPAddress,Username, InstituteId)
                                           VALUES 
                        ( @EmployeeCount ,@DownloadDate ,@IPAddress,@Username, @InstituteId)";
                    await _connection.ExecuteAsync(historyQuery, new
                    {
                        EmployeeCount = rowsAffected,
                        DownloadDate = DateTime.Now,
                        IPAddress = ipAddress,
                        Username = "", // Assuming you have this in the request
                        InstituteId = InstituteId
                    });
                    if (rowsAffected > 0)
                    {
                        // Additional logic to handle related entities (family, documents, qualifications, etc.)
                        employee.Family.Employee_id = employee.Employee_id;

                        var empfam = await AddUpdateEmployeeFamily(employee.Family ?? new EmployeeFamily());
                        var empdoc = await AddUpdateEmployeeDocuments(employee.EmployeeDocuments ?? new List<EmployeeDocument>(), employee.Employee_id);
                        var empQua = await AddUpdateEmployeeQualification(employee.EmployeeQualifications ?? new List<EmployeeQualification>(), employee.Employee_id);
                        var empwork = await AddUpdateEmployeeWorkExp(employee.EmployeeWorkExperiences ?? new List<EmployeeWorkExperience>(), employee.Employee_id);
                        var empbank = await AddUpdateEmployeeBankDetails(employee.EmployeeBankDetails, employee.Employee_id);
                        var empadd = await AddUpdateEmployeeAddressDetails(employee.EmployeeAddressDetails, employee.Employee_id);

                        employee.EmployeeStaffMappingRequest.EmployeeId = employee.Employee_id;
                        var mapp = await AddUpdateEmployeeStaffMapping(employee.EmployeeStaffMappingRequest);
                    }
                    else
                    {
                        // If any employee fails to update, return failure response
                        return new ServiceResponse<string>(false, $"Failed to update Employee with ID {employee.Employee_id}", string.Empty, 500);
                    }
                }

                // If all employees are successfully updated
                return new ServiceResponse<string>(true, "All employees updated successfully", string.Empty, 200);
            }
            catch (Exception ex)
            {
                // Handle exception
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateEmployeeFamily(EmployeeFamily request)
        {
            try
            {
                int rowsAffected;
                if (request.Employee_family_id == 0)
                {
                    string sql = @"INSERT INTO [dbo].[tbl_EmployeeFamilyMaster] 
                        (Employee_id, Father_Name, Fathers_Occupation, Mother_Name, Mothers_Occupation, 
                         Spouse_Name, Spouses_Occupation, Guardian_Name, Guardians_Occupation, 
                         Primary_Emergency_Contact_no, Secondary_Emergency_Contact_no) 
                       VALUES 
                        (@Employee_id, @Father_Name, @Fathers_Occupation, @Mother_Name, @Mothers_Occupation, 
                         @Spouse_Name, @Spouses_Occupation, @Guardian_Name, @Guardians_Occupation, 
                         @Primary_Emergency_Contact_no, @Secondary_Emergency_Contact_no);
                       SELECT SCOPE_IDENTITY();";
                    // Execute the query and retrieve the inserted Employee_family_id
                    rowsAffected = await _connection.ExecuteAsync(sql, new
                    {
                        request.Employee_id,
                        request.Father_Name,
                        request.Fathers_Occupation,
                        request.Mother_Name,
                        request.Mothers_Occupation,
                        request.Spouse_Name,
                        request.Spouses_Occupation,
                        request.Guardian_Name,
                        request.Guardians_Occupation,
                        request.Primary_Emergency_Contact_no,
                        request.Secondary_Emergency_Contact_no
                    });
                }
                else
                {
                    string sql = @"UPDATE [dbo].[tbl_EmployeeFamilyMaster] SET 
                        Father_Name = @Father_Name, Fathers_Occupation = @Fathers_Occupation, 
                        Mother_Name = @Mother_Name, Mothers_Occupation = @Mothers_Occupation, 
                        Spouse_Name = @Spouse_Name, Spouses_Occupation = @Spouses_Occupation, 
                        Guardian_Name = @Guardian_Name, Guardians_Occupation = @Guardians_Occupation, 
                        Primary_Emergency_Contact_no = @Primary_Emergency_Contact_no, 
                        Secondary_Emergency_Contact_no = @Secondary_Emergency_Contact_no 
                      WHERE Employee_family_id = @Employee_family_id";

                    // Execute the query
                    rowsAffected = await _connection.ExecuteAsync(sql, new
                    {
                        request.Father_Name,
                        request.Fathers_Occupation,
                        request.Mother_Name,
                        request.Mothers_Occupation,
                        request.Spouse_Name,
                        request.Spouses_Occupation,
                        request.Guardian_Name,
                        request.Guardians_Occupation,
                        request.Primary_Emergency_Contact_no,
                        request.Secondary_Emergency_Contact_no,
                        request.Employee_family_id
                    });
                }
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<int>(true, "Record added successfully", rowsAffected, 200);
                }
                else
                {
                    return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateEmployeeStaffMapping(EmployeeStaffMappingRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Remove existing records for the EmployeeId in tbl_EmployeeStaffMapClassTeacher
                if (request.EmployeeStaffMappingsClassTeacher != null)
                {
                    await connection.ExecuteAsync(
                        "DELETE FROM tbl_EmployeeStaffMapClassTeacher WHERE EmployeeId = @EmployeeId",
                        new { request.EmployeeId }
                    );

                    // Insert new record for EmployeeStaffMappingsClassTeacher
                    await connection.ExecuteAsync(
                        @"INSERT INTO tbl_EmployeeStaffMapClassTeacher (EmployeeId, ClassId, SectionId, SubjectId)
                VALUES (@EmployeeId, @ClassId, @SectionId, @SubjectId)",
                        new
                        {
                            request.EmployeeId,
                            request.EmployeeStaffMappingsClassTeacher.ClassId,
                            request.EmployeeStaffMappingsClassTeacher.SectionId,
                            request.EmployeeStaffMappingsClassTeacher.SubjectId
                        }
                    );
                }

                // Remove existing records for the EmployeeId in tbl_EmployeeStappMapClassSection
                if (request.EmployeeStappMappingsClassSection != null)
                {
                    await connection.ExecuteAsync(
                        "DELETE FROM tbl_EmployeeStappMapClassSection WHERE EmployeeId = @EmployeeId",
                        new { request.EmployeeId }
                    );

                    // Insert new record for EmployeeStappMappingsClassSection
                    await connection.ExecuteAsync(
                        @"INSERT INTO tbl_EmployeeStappMapClassSection (EmployeeId, ClassId, SectionId, SubjectId)
                VALUES (@EmployeeId, @ClassId, @SectionId, @SubjectId)",
                        new
                        {
                            request.EmployeeId,
                            request.EmployeeStappMappingsClassSection.ClassId,
                            request.EmployeeStappMappingsClassSection.SectionId,
                            request.EmployeeStappMappingsClassSection.SubjectId
                        }
                    );
                }

                return new ServiceResponse<int>(true, "Mapping saved successfully", 0, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateEmployeeDocuments(List<EmployeeDocument> request, int employee_id)
        {
            try
            {
                foreach (var data in request)
                {
                    data.employee_id = employee_id;
                }
                int addedRecords = 0;
                string query = "SELECT COUNT(*) FROM tbl_DocumentsMaster WHERE employee_id = @employee_id";
                int count = await _connection.ExecuteScalarAsync<int>(query, new { employee_id });

                // If records exist, delete them and insert new ones
                if (count > 0)
                {
                    string deleteQuery = "DELETE FROM tbl_DocumentsMaster WHERE employee_id = @employee_id";
                    int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { employee_id });
                    if (rowsAffected > 0)
                    {
                        string insertQuery = @"INSERT INTO [dbo].[tbl_DocumentsMaster] 
                    (employee_id, Document_Name, file_path) 
                    VALUES 
                    (@employee_id, @Document_Name, @file_path);";

                        foreach (EmployeeDocument document in request)
                        {
                            document.file_path = ImageUpload(document.file_path);
                        }

                        addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                        return new ServiceResponse<int>(true, "Documents updated successfully", addedRecords, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Error occurred while deleting existing documents", 0, 500);
                    }
                }
                else
                {
                    // If no records exist, insert new documents
                    string insertQuery = @"INSERT INTO [dbo].[tbl_DocumentsMaster] 
                (employee_id, Document_Name, file_path) 
                VALUES 
                (@employee_id, @Document_Name, @file_path);";

                    foreach (EmployeeDocument document in request)
                    {
                        document.file_path = ImageUpload(document.file_path);
                    }

                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                    return new ServiceResponse<int>(true, "Documents added successfully", addedRecords, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateEmployeeQualification(List<EmployeeQualification>? request, int employeeId)
        {
            try
            {
                int addedRecords = 0;
                if (request != null)
                {
                    foreach (var data in request)
                    {
                        data.employee_id = employeeId;
                    }
                }
                string query = "SELECT COUNT(*) FROM tbl_QualificationInfoMaster WHERE employee_id = @employee_id";
                int count = await _connection.ExecuteScalarAsync<int>(query, new { employee_id = employeeId });
                if (count > 0)
                {
                    string deleteQuery = "DELETE FROM tbl_QualificationInfoMaster WHERE employee_id = @employee_id";
                    int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { employee_id = employeeId });
                    if (rowsAffected > 0)
                    {

                        string insertQuery = @"INSERT INTO [dbo].[tbl_QualificationInfoMaster] 
                        (employee_id, Educational_Qualification, Year_of_Completion) 
                       VALUES 
                        (@employee_id, @Educational_Qualification, @Year_of_Completion);";

                        // Execute the query with multiple parameterized sets of values
                        addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                    }
                    if (addedRecords > 0)
                    {
                        return new ServiceResponse<int>(true, "Records added successfully", addedRecords, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "some error occured", 0, 500);
                    }
                }
                else
                {
                    string insertQuery = @"INSERT INTO [dbo].[tbl_QualificationInfoMaster] 
                        (employee_id, Educational_Qualification, Year_of_Completion) 
                       VALUES 
                        (@employee_id, @Educational_Qualification, @Year_of_Completion);";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                    if (addedRecords > 0)
                    {
                        return new ServiceResponse<int>(true, "Records added successfully", addedRecords, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "some error occured", 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateEmployeeWorkExp(List<EmployeeWorkExperience>? request, int employeeId)
        {
            try
            {
                int addedRecords = 0;
                if (request != null)
                {
                    foreach (var data in request)
                    {
                        data.Employee_id = employeeId;
                    }
                }
                string query = "SELECT COUNT(*) FROM tbl_WorkExperienceMaster WHERE employee_id = @employee_id";
                int count = await _connection.ExecuteScalarAsync<int>(query, new { employee_id = employeeId });
                if (count > 0)
                {
                    string deleteQuery = "DELETE FROM tbl_WorkExperienceMaster WHERE employee_id = @employee_id";
                    int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { employee_id = employeeId });
                    if (rowsAffected > 0)
                    {

                        string insertQuery = @"INSERT INTO [dbo].[tbl_WorkExperienceMaster] 
                        (Year, Month, Previous_Organisation, Previous_Designation, Employee_id) 
                       VALUES 
                        (@Year, @Month, @Previous_Organisation, @Previous_Designation, @Employee_id);";

                        // Execute the query with multiple parameterized sets of values
                        addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                    }
                    if (addedRecords > 0)
                    {
                        return new ServiceResponse<int>(true, "Records added successfully", addedRecords, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "some error occured", 0, 500);
                    }
                }
                else
                {
                    string insertQuery = @"INSERT INTO [dbo].[tbl_WorkExperienceMaster] 
                        (Year, Month, Previous_Organisation, Previous_Designation, Employee_id) 
                       VALUES 
                        (@Year, @Month, @Previous_Organisation, @Previous_Designation, @Employee_id);";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                    if (addedRecords > 0)
                    {
                        return new ServiceResponse<int>(true, "Records added successfully", addedRecords, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "some error occured", 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateEmployeeBankDetails(List<EmployeeBankDetails>? request, int employeeId)
        {
            try
            {
                int addedRecords = 0;
                if (request != null)
                {
                    foreach (var data in request)
                    {
                        data.employee_id = employeeId;
                    }
                }
                string query = "SELECT COUNT(*) FROM tbl_BankDetailsmaster WHERE employee_id = @employee_id";
                int count = await _connection.ExecuteScalarAsync<int>(query, new { employee_id = employeeId });
                if (count > 0)
                {
                    string deleteQuery = "DELETE FROM tbl_BankDetailsmaster WHERE employee_id = @employee_id";
                    int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { employee_id = employeeId });
                    if (rowsAffected > 0)
                    {

                        string insertQuery = @"INSERT INTO [dbo].[tbl_BankDetailsmaster] 
                        (employee_id, bank_name, account_name, account_number, IFSC_code, Bank_address) 
                       VALUES 
                        (@employee_id, @bank_name, @account_name, @account_number, @IFSC_code, @Bank_address);";
                        // Execute the query with multiple parameterized sets of values
                        addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                    }
                    if (addedRecords > 0)
                    {
                        return new ServiceResponse<int>(true, "Records added successfully", addedRecords, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "some error occured", 0, 500);
                    }
                }
                else
                {
                    string insertQuery = @"INSERT INTO [dbo].[tbl_BankDetailsmaster] 
                        (employee_id, bank_name, account_name, account_number, IFSC_code, Bank_address) 
                       VALUES 
                        (@employee_id, @bank_name, @account_name, @account_number, @IFSC_code, @Bank_address);"; // Retrieve the inserted bank_id

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                    if (addedRecords > 0)
                    {
                        return new ServiceResponse<int>(true, "Records added successfully", addedRecords, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "some error occured", 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateEmployeeAddressDetails(List<EmployeeAddressDetails>? request, int employeeId)
        {
            if (request == null || !request.Any())
            {
                return new ServiceResponse<int>(false, "Request cannot be null or empty.", 0, 400);
            }

            // Set Employee_id for each address in the request
            foreach (var data in request)
            {
                data.Employee_id = employeeId;
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }


            try
            {
                var connection = new SqlConnection(_connectionString);
                // Delete existing addresses for the employee
                string deleteAddressSql = @"
                DELETE FROM tbl_EmployeePresentAddress 
                WHERE Employee_id = @EmployeeId;";

                await connection.ExecuteAsync(deleteAddressSql, new { EmployeeId = employeeId });

                // Insert new addresses
                string insertAddressSql = @"
                INSERT INTO tbl_EmployeePresentAddress (Address, CountryName, StateName, CityName, DistrictName, Pin_code, AddressTypeId, Employee_id)
                VALUES (@Address, @CountryName, @StateName, @CityName, @DistrictName, @Pin_code, @AddressTypeId, @Employee_id);";

                // Execute the insert operation for each address
                foreach (var address in request)
                {
                    await connection.ExecuteAsync(insertAddressSql, address);
                }

            

                return new ServiceResponse<int>(true, "Address saved successfully.", request.Count, 200);
            }
            catch (Exception ex)
            {
               
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<EmployeeProfileResponseDTO>> GetEmployeeProfileById(int employeeId)
        {
            try
            {
                var response = new EmployeeProfileResponseDTO();
                string sql = @"
            SELECT ep.Employee_id,
                   ep.First_Name,
                   ep.Middle_Name,
                   ep.Last_Name,
                   ep.Gender_id,
                   g.Gender_Type as GenderName,
                   ep.Department_id,
                   ep.Designation_id,
                   ep.mobile_number,
                   ep.Date_of_Joining,
                   ep.Nationality_id,
                   ep.Religion_id,
                   ep.Date_of_Birth,
                   ep.EmailID,
                   ep.Employee_code_id,
                   ep.marrital_status_id,
                   ep.Blood_Group_id,
                   ep.aadhar_no,
                   ep.pan_no,
                   ep.EPF_no,
                   ep.ESIC_no,
                   ep.Institute_id,
                   ep.EmpPhoto,
                   ep.uan_no,
                   ep.Status,
                   d.DepartmentName,
                   des.DesignationName,
                   n.Nationality_Type as NationalityName,
                   r.Religion_Type as ReligionName,
                   ms.StatusName as MaritalStatusName,
                   bg.Blood_Group_Type as BloodGroupName
            FROM [dbo].[tbl_EmployeeProfileMaster] ep
            LEFT JOIN [dbo].[tbl_Department] d ON ep.Department_id = d.Department_id
            LEFT JOIN [dbo].[tbl_Designation] des ON ep.Designation_id = des.Designation_id
            LEFT JOIN [dbo].[tbl_Nationality] n ON ep.Nationality_id = n.Nationality_id
            LEFT JOIN [dbo].[tbl_Religion] r ON ep.Religion_id = r.Religion_id
            LEFT JOIN [dbo].[tbl_MaritalStatus] ms ON ep.marrital_status_id = ms.statusId
            LEFT JOIN [dbo].[tbl_BloodGroup] bg ON ep.Blood_Group_id = bg.Blood_Group_id
            LEFT JOIN tbl_Gender g on ep.Gender_id = g.Gender_id
            WHERE ep.Employee_id = @EmployeeId";

                // Execute the query and retrieve the employee profile
                var employee = await _connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { EmployeeId = employeeId });
                if (employee != null)
                {
                    response.aadhar_no = employee.aadhar_no;
                    response.pan_no = employee.pan_no;
                    response.ESIC_no = employee.ESIC_no;
                    response.EPF_no = employee.EPF_no;
                    response.Blood_Group_id = employee.Blood_Group_id;
                    response.Date_of_Birth = employee.Date_of_Birth;
                    response.Date_of_Joining = employee.Date_of_Joining;
                    response.Department_id = employee.Department_id;
                    response.Designation_id = employee.Designation_id;
                    response.EmailID = employee.EmailID;
                    response.Employee_code_id = employee.Employee_code_id;
                    response.Religion_id = employee.Religion_id;
                    response.Employee_id = employee.Employee_id;
                    response.First_Name = employee.First_Name;
                    response.Gender_id = employee.Gender_id;
                    response.Institute_id = employee.Institute_id;
                    response.Last_Name = employee.Last_Name;
                    response.marrital_status_id = employee.marrital_status_id;
                    response.Middle_Name = employee.Middle_Name;
                    response.mobile_number = employee.mobile_number;
                    response.Nationality_id = employee.Nationality_id;
                    response.EmpPhoto = GetImage(employee.EmpPhoto);
                    response.uan_no = employee.uan_no;

                    // Set additional name properties
                    response.DepartmentName = employee.DepartmentName;
                    response.DesignationName = employee.DesignationName;
                    response.NationalityName = employee.NationalityName;
                    response.ReligionName = employee.ReligionName;
                    response.MaritalStatusName = employee.MaritalStatusName;
                    response.BloodGroupName = employee.BloodGroupName;
                    response.GenderName = employee.GenderName;
                    string famsql = @"SELECT Employee_family_id, Employee_id, Father_Name, Fathers_Occupation,
                                    Mother_Name, Mothers_Occupation, Spouse_Name, Spouses_Occupation,
                                    Guardian_Name, Guardians_Occupation, Primary_Emergency_Contact_no,
                                    Secondary_Emergency_Contact_no
                              FROM [dbo].[tbl_EmployeeFamilyMaster]
                              WHERE Employee_id = @EmployeeId";

                    // Execute the query and retrieve the employee family details
                    var employeeFamily = await _connection.QueryFirstOrDefaultAsync<EmployeeFamily>(famsql, new { EmployeeId = employeeId });
                    if (employeeFamily != null)
                    {
                        response.Family = employeeFamily;
                    }

                    string banksql = @"SELECT bank_id, employee_id, bank_name, account_name, account_number, IFSC_code, Bank_address
                               FROM [dbo].[tbl_BankDetailsmaster]
                               WHERE employee_id = @EmployeeId";

                    // Execute the query and retrieve the list of bank details
                    var bankDetails = await _connection.QueryAsync<EmployeeBankDetails>(banksql, new { EmployeeId = employeeId });
                    if (bankDetails != null)
                    {
                        response.EmployeeBankDetails = bankDetails.AsList();
                    }

                    string docsql = @"SELECT Document_id, employee_id, Document_Name, file_path
                              FROM [dbo].[tbl_DocumentsMaster]
                              WHERE employee_id = @EmployeeId";

                    // Execute the query and retrieve the list of documents
                    var documents = await _connection.QueryAsync<EmployeeDocument>(docsql, new { EmployeeId = employeeId });
                    if (documents != null)
                    {
                        foreach (var item in documents)
                        {
                            item.file_path = GetImage(item.file_path);
                        }
                        response.EmployeeDocuments = documents.AsList();
                    }

                    string quasql = @"SELECT Qualification_Info_id, employee_id, Educational_Qualification, Year_of_Completion
                              FROM [dbo].[tbl_QualificationInfoMaster]
                              WHERE employee_id = @EmployeeId";

                    // Execute the query and retrieve the list of qualifications
                    var qualifications = await _connection.QueryAsync<EmployeeQualification>(quasql, new { EmployeeId = employeeId });
                    if (qualifications != null)
                    {
                        response.EmployeeQualifications = qualifications.AsList();
                    }

                    string expsql = @"SELECT work_experience_id, Year, Month, Previous_Organisation, Previous_Designation, Employee_id
                              FROM [dbo].[tbl_WorkExperienceMaster]
                              WHERE Employee_id = @EmployeeId";

                    // Execute the query and retrieve the list of work experiences
                    var workExperiences = await _connection.QueryAsync<EmployeeWorkExperience>(expsql, new { EmployeeId = employeeId });
                    if (workExperiences != null)
                    {
                        response.EmployeeWorkExperiences = workExperiences.AsList();
                    }
                    var data = await GetEmployeeAddressDetailsById(employeeId);
                    response.EmployeeAddressDetails = data.Data;
                    var mapping = await GetEmployeeMappingById(employeeId);
                    response.EmployeeStaffMappingResponse = mapping.Data;
                    return new ServiceResponse<EmployeeProfileResponseDTO>(true, "Records found", response, 200);
                }
                else
                {
                    return new ServiceResponse<EmployeeProfileResponseDTO>(false, "Records not found", new EmployeeProfileResponseDTO(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeProfileResponseDTO>(false, ex.Message, new EmployeeProfileResponseDTO(), 500);
            }
        }
        public async Task<ServiceResponse<EmployeeFamily>> GetEmployeeFamilyDetailsById(int employeeId)
        {
            try
            {
                string query = @"
                SELECT 
                    [Employee_family_id],
                    [Employee_id],
                    [Father_Name],
                    [Fathers_Occupation],
                    [Mother_Name],
                    [Mothers_Occupation],
                    [Spouse_Name],
                    [Spouses_Occupation],
                    [Guardian_Name],
                    [Guardians_Occupation],
                    [Primary_Emergency_Contact_no],
                    [Secondary_Emergency_Contact_no]
                FROM 
                    [tbl_EmployeeFamilyMaster]
                WHERE 
                    [Employee_id] = @EmployeeId";

                var data = await _connection.QueryFirstOrDefaultAsync<EmployeeFamily>(query, new { EmployeeId = employeeId });
                if (data != null)
                {
                    return new ServiceResponse<EmployeeFamily>(true, "Records found", data, 200);
                }
                else
                {
                    return new ServiceResponse<EmployeeFamily>(false, "No records found", new EmployeeFamily(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeFamily>(false, ex.Message, new EmployeeFamily(), 500);
            }
        }
        public async Task<ServiceResponse<IEnumerable<dynamic>>> GetEmployeeProfileList(GetAllEmployeeListRequest request)
        {
            try
            {
                // Fetch active columns
                var activeColumns = await GetActiveColumns(request.ActiveColumns);
                if (activeColumns == null || !activeColumns.Any())
                {
                    return new ServiceResponse<IEnumerable<dynamic>>(false, "No active columns found", new List<dynamic>(), 204);
                }

                // Initialize parameters
                var parameters = new DynamicParameters();
                parameters.Add("InstituteId", request.InstituteId);

                // Add filtering conditions
                if (request.DepartmentId > 0)
                {
                    parameters.Add("DepartmentId", request.DepartmentId);
                }

                if (request.DesignationId > 0)
                {
                    parameters.Add("DesignationId", request.DesignationId);
                }

                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    parameters.Add("SearchText", $"%{request.SearchText}%");
                }

                // Initialize StringBuilder for building the SQL query
                var sqlBuilder = new StringBuilder("SELECT DISTINCT ep.Employee_id");

                // Build dynamic SQL based on active columns
                foreach (var column in activeColumns)
                {
                    switch (column.CategoryId)
                    {
                        case 1: // tbl_EmployeeProfileMaster
                            sqlBuilder.Append($", ep.{column.ColumnDatabaseName}");
                            break;
                        case 2: // tbl_EmployeePresentAddress
                            sqlBuilder.Append($", pa.{column.ColumnDatabaseName}");
                            break;
                        case 3: // tbl_EmployeeFamilyMaster
                            sqlBuilder.Append($", ef.{column.ColumnDatabaseName}");
                            break;
                        case 4: // tbl_QualificationInfoMaster
                            sqlBuilder.Append($", qi.{column.ColumnDatabaseName}");
                            break;
                        case 5: // tbl_WorkExperienceMaster
                            sqlBuilder.Append($", we.{column.ColumnDatabaseName}");
                            break;
                        case 6: // tbl_BankDetailsMaster
                            sqlBuilder.Append($", bd.{column.ColumnDatabaseName}");
                            break;
                        default:
                            throw new Exception($"Unknown CategoryId {column.CategoryId}");
                    }
                }
                sqlBuilder.Append(@"
            , d.DepartmentName
            , des.DesignationName
            , g.Gender_Type
            , n.Nationality_Type
            , r.Religion_Type
            , ms.StatusName
            , bg.Blood_Group_Type");
                // Build FROM and JOIN clauses
                sqlBuilder.Append(@"
        FROM [dbo].[tbl_EmployeeProfileMaster] ep
        LEFT JOIN [dbo].[tbl_EmployeePresentAddress] pa ON ep.Employee_id = pa.Employee_id
        LEFT JOIN [dbo].[tbl_EmployeeFamilyMaster] ef ON ep.Employee_id = ef.Employee_id
        LEFT JOIN [dbo].[tbl_QualificationInfoMaster] qi ON ep.Employee_id = qi.Employee_id
        LEFT JOIN [dbo].[tbl_WorkExperienceMaster] we ON ep.Employee_id = we.Employee_id
        LEFT JOIN [dbo].[tbl_BankDetailsMaster] bd ON ep.Employee_id = bd.Employee_id
        LEFT JOIN [dbo].[tbl_Department] d ON ep.Department_id = d.Department_id 
        LEFT JOIN [dbo].[tbl_Designation] des ON ep.Designation_id = des.Designation_id 
        LEFT JOIN [dbo].[tbl_Nationality] n ON ep.Nationality_id = n.Nationality_id 
        LEFT JOIN [dbo].[tbl_Religion] r ON ep.Religion_id = r.Religion_id 
        LEFT JOIN [dbo].[tbl_MaritalStatus] ms ON ep.marrital_status_id = ms.statusId 
        LEFT JOIN [dbo].[tbl_BloodGroup] bg ON ep.Blood_Group_id = bg.Blood_Group_id 
        LEFT JOIN tbl_Gender g on ep.Gender_id = g.Gender_id 
        WHERE ep.Institute_id = @InstituteId");

                // Add filtering conditions to the SQL query
                if (request.DepartmentId > 0)
                {
                    sqlBuilder.Append(" AND ep.Department_id = @DepartmentId");
                }
                if (request.DesignationId > 0)
                {
                    sqlBuilder.Append(" AND ep.Designation_id = @DesignationId");
                }
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    sqlBuilder.Append(" AND (ep.First_Name LIKE @SearchText OR ep.Last_Name LIKE @SearchText OR ep.EmailID LIKE @SearchText)");
                }

                // Execute the query
                var employeeData = await _connection.QueryAsync<dynamic>(sqlBuilder.ToString(), parameters);

                // Pagination logic
                var paginatedResults = employeeData
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                // Initialize dynamic response
                var response = new List<ExpandoObject>();

                foreach (var employee in paginatedResults)
                {
                    dynamic dynamicEmployee = new ExpandoObject();
                    var employeeDict = (IDictionary<string, object>)dynamicEmployee;

                    // Add properties dynamically based on active columns
                    foreach (var column in activeColumns)
                    {
                        var columnName = column.ColumnDatabaseName;
                        if (((IDictionary<string, object>)employee).ContainsKey(columnName))
                        {
                            employeeDict[columnName] = ((IDictionary<string, object>)employee)[columnName];
                        }
                    }

                    // Conditionally map these specific fields if related columns are part of activeColumns
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Department_id"))
                    {
                        employeeDict["DepartmentName"] = employee.DepartmentName;
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Designation_id"))
                    {
                        employeeDict["DesignationName"] = employee.DesignationName;
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Gender_id"))
                    {
                        employeeDict["GenderName"] = employee.Gender_Type;
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Nationality_id"))
                    {
                        employeeDict["NationalityName"] = employee.Nationality_Type;
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Religion_id"))
                    {
                        employeeDict["ReligionName"] = employee.Religion_Type;
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "marrital_status_id"))
                    {
                        employeeDict["MaritalStatusName"] = employee.StatusName;
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Blood_Group_id"))
                    {
                        employeeDict["BloodGroupName"] = employee.Blood_Group_Type;
                    }
                    response.Add(dynamicEmployee);
                }

                if (response.Any())
                {
                    return new ServiceResponse<IEnumerable<dynamic>>(true, "Records found", response, 200, response.Count);
                }
                else
                {
                    return new ServiceResponse<IEnumerable<dynamic>>(false, "No records found", new List<dynamic>(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<dynamic>>(false, ex.Message, new List<dynamic>(), 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> BulkUpdate(GetListRequest request)
        {
            try
            {
                // Fetch active columns
                var activeColumns = await GetActiveColumns(request.ActiveColumns);
                if (activeColumns == null || !activeColumns.Any())
                {
                    return new ServiceResponse<byte[]>(false, "No active columns found", null, 204);
                }

                // Initialize parameters
                var parameters = new DynamicParameters();
                parameters.Add("InstituteId", request.InstituteId);

                // Add filtering conditions
                if (request.DepartmentId > 0)
                {
                    parameters.Add("DepartmentId", request.DepartmentId);
                }

                if (request.DesignationId > 0)
                {
                    parameters.Add("DesignationId", request.DesignationId);
                }

                // Build SQL query dynamically
                var sqlBuilder = new StringBuilder("SELECT DISTINCT ep.Employee_id");

                foreach (var column in activeColumns)
                {
                    switch (column.CategoryId)
                    {
                        case 1: // tbl_EmployeeProfileMaster
                            sqlBuilder.Append($", ep.{column.ColumnDatabaseName}");
                            break;
                        case 2: // tbl_EmployeePresentAddress
                            sqlBuilder.Append($", pa.{column.ColumnDatabaseName}");
                            break;
                        case 3: // tbl_EmployeeFamilyMaster
                            sqlBuilder.Append($", ef.{column.ColumnDatabaseName}");
                            break;
                        case 4: // tbl_QualificationInfoMaster
                            sqlBuilder.Append($", qi.{column.ColumnDatabaseName}");
                            break;
                        case 5: // tbl_WorkExperienceMaster
                            sqlBuilder.Append($", we.{column.ColumnDatabaseName}");
                            break;
                        case 6: // tbl_BankDetailsMaster
                            sqlBuilder.Append($", bd.{column.ColumnDatabaseName}");
                            break;
                        default:
                            throw new Exception($"Unknown CategoryId {column.CategoryId}");
                    }
                }

                // Add static columns
                sqlBuilder.Append(@"
            , d.DepartmentName
            , des.DesignationName
            , g.Gender_Type
            , n.Nationality_Type
            , r.Religion_Type
            , ms.StatusName
            , bg.Blood_Group_Type");

                // FROM and JOIN clauses
                sqlBuilder.Append(@"
        FROM [dbo].[tbl_EmployeeProfileMaster] ep
        LEFT JOIN [dbo].[tbl_EmployeePresentAddress] pa ON ep.Employee_id = pa.Employee_id
        LEFT JOIN [dbo].[tbl_EmployeeFamilyMaster] ef ON ep.Employee_id = ef.Employee_id
        LEFT JOIN [dbo].[tbl_QualificationInfoMaster] qi ON ep.Employee_id = qi.Employee_id
        LEFT JOIN [dbo].[tbl_WorkExperienceMaster] we ON ep.Employee_id = we.Employee_id
        LEFT JOIN [dbo].[tbl_BankDetailsMaster] bd ON ep.Employee_id = bd.Employee_id
        LEFT JOIN [dbo].[tbl_Department] d ON ep.Department_id = d.Department_id 
        LEFT JOIN [dbo].[tbl_Designation] des ON ep.Designation_id = des.Designation_id 
        LEFT JOIN [dbo].[tbl_Nationality] n ON ep.Nationality_id = n.Nationality_id 
        LEFT JOIN [dbo].[tbl_Religion] r ON ep.Religion_id = r.Religion_id 
        LEFT JOIN [dbo].[tbl_MaritalStatus] ms ON ep.marrital_status_id = ms.statusId 
        LEFT JOIN [dbo].[tbl_BloodGroup] bg ON ep.Blood_Group_id = bg.Blood_Group_id 
        LEFT JOIN tbl_Gender g on ep.Gender_id = g.Gender_id 
        WHERE ep.Institute_id = @InstituteId");

                // Add filters if needed
                if (request.DepartmentId > 0)
                {
                    sqlBuilder.Append(" AND ep.Department_id = @DepartmentId");
                }
                if (request.DesignationId > 0)
                {
                    sqlBuilder.Append(" AND ep.Designation_id = @DesignationId");
                }

                // Execute the query
                var employeeData = await _connection.QueryAsync<dynamic>(sqlBuilder.ToString(), parameters);

                // Fetch master data for additional sheets
                var genderData = await _connection.QueryAsync<dynamic>("SELECT Gender_id, Gender_Type FROM [dbo].[tbl_Gender]");
                var departmentData = await _connection.QueryAsync<dynamic>("SELECT Department_id, DepartmentName FROM [dbo].[tbl_Department] WHERE Institute_id = @InstituteId", parameters);
                var designationData = await _connection.QueryAsync<dynamic>("SELECT Designation_id, DesignationName FROM [dbo].[tbl_Designation] WHERE Institute_id = @InstituteId", parameters);
                var nationalityData = await _connection.QueryAsync<dynamic>("SELECT Nationality_id, Nationality_Type FROM [dbo].[tbl_Nationality]");
                var religionData = await _connection.QueryAsync<dynamic>("SELECT Religion_id, Religion_Type FROM [dbo].[tbl_Religion]");
                var maritalStatusData = await _connection.QueryAsync<dynamic>("SELECT statusId, StatusName FROM [dbo].[tbl_MaritalStatus]");
                var bloodGroupData = await _connection.QueryAsync<dynamic>("SELECT Blood_Group_id, Blood_Group_Type FROM [dbo].[tbl_BloodGroup]");

                // Initialize Excel package
                using (var package = new ExcelPackage())
                {
                    // Main sheet for employee data
                    var worksheet = package.Workbook.Worksheets.Add("Employee Data");
                    int columnIndex = 1;

                    // Write column headers dynamically
                    foreach (var column in activeColumns)
                    {
                        worksheet.Cells[1, columnIndex].Value = column.ColumnDatabaseName;
                        columnIndex++;
                    }

                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Department_id"))
                    {
                        worksheet.Cells[1, columnIndex++].Value = "DepartmentName";
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Designation_id"))
                    {
                        worksheet.Cells[1, columnIndex++].Value = "DesignationName";
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Gender_id"))
                    {
                        worksheet.Cells[1, columnIndex++].Value = "GenderName";
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Nationality_id"))
                    {
                        worksheet.Cells[1, columnIndex++].Value = "NationalityName";
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Religion_id"))
                    {
                        worksheet.Cells[1, columnIndex++].Value = "ReligionName";
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "marrital_status_id"))
                    {
                        worksheet.Cells[1, columnIndex++].Value = "MaritalStatusName";
                    }
                    if (activeColumns.Any(c => c.ColumnDatabaseName == "Blood_Group_id"))
                    {
                        worksheet.Cells[1, columnIndex++].Value = "BloodGroupName";
                    }

                    // Write employee data rows
                    int rowIndex = 2;
                    foreach (var employee in employeeData)
                    {
                        columnIndex = 1;
                        foreach (var column in activeColumns)
                        {
                            var columnName = column.ColumnDatabaseName;
                            worksheet.Cells[rowIndex, columnIndex].Value = ((IDictionary<string, object>)employee)[columnName]?.ToString();
                            columnIndex++;
                        }

                        if (activeColumns.Any(c => c.ColumnDatabaseName == "Department_id"))
                        {
                            worksheet.Cells[rowIndex, columnIndex++].Value = employee.DepartmentName;
                        }
                        if (activeColumns.Any(c => c.ColumnDatabaseName == "Designation_id"))
                        {
                            worksheet.Cells[rowIndex, columnIndex++].Value = employee.DesignationName;
                        }
                        if (activeColumns.Any(c => c.ColumnDatabaseName == "Gender_id"))
                        {
                            worksheet.Cells[rowIndex, columnIndex++].Value = employee.Gender_Type;
                        }
                        if (activeColumns.Any(c => c.ColumnDatabaseName == "Nationality_id"))
                        {
                            worksheet.Cells[rowIndex, columnIndex++].Value = employee.Nationality_Type;
                        }
                        if (activeColumns.Any(c => c.ColumnDatabaseName == "Religion_id"))
                        {
                            worksheet.Cells[rowIndex, columnIndex++].Value = employee.Religion_Type;
                        }
                        if (activeColumns.Any(c => c.ColumnDatabaseName == "marrital_status_id"))
                        {
                            worksheet.Cells[rowIndex, columnIndex++].Value = employee.StatusName;
                        }
                        if (activeColumns.Any(c => c.ColumnDatabaseName == "Blood_Group_id"))
                        {
                            worksheet.Cells[rowIndex, columnIndex++].Value = employee.Blood_Group_Type;
                        }
                        rowIndex++;
                    }

                    // Create master data sheets
                    AddMasterSheet(package, "Gender Data", new[] { "Gender_id", "Gender_Type" }, genderData);
                    AddMasterSheet(package, "Department Data", new[] { "Department_id", "DepartmentName" }, departmentData);
                    AddMasterSheet(package, "Designation Data", new[] { "Designation_id", "DesignationName" }, designationData);
                    AddMasterSheet(package, "Nationality Data", new[] { "Nationality_id", "Nationality_Type" }, nationalityData);
                    AddMasterSheet(package, "Religion Data", new[] { "Religion_id", "Religion_Type" }, religionData);
                    AddMasterSheet(package, "Marital Status Data", new[] { "statusId", "StatusName" }, maritalStatusData);
                    AddMasterSheet(package, "Blood Group Data", new[] { "Blood_Group_id", "Blood_Group_Type" }, bloodGroupData);

                    // Save the Excel package to a byte array
                    var excelFile = package.GetAsByteArray();
                    return new ServiceResponse<byte[]>(true, "Success", excelFile, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<IEnumerable<CategoryWiseEmployeeColumns>>> GetEmployeeColumnsAsync()
        {
            string query = @"
    SELECT TOP (1000) [ECMId],
                      [ColumnDisplayName],
                      [ColumnDatabaseName],
                      [CategoryId],
                      [Status]
    FROM [iGuruERP].[dbo].[tbl_EmployeeColumnMaster]";

            var data = await _connection.QueryAsync<EmployeeColumn>(query);

            // Group the data by CategoryId
            var groupedData = data
                .GroupBy(c => c.CategoryId)
                .Select(g => new CategoryWiseEmployeeColumns
                {
                    CategoryId = g.Key,
                    EmployeeColumns = g.ToList()
                });

            return new ServiceResponse<IEnumerable<CategoryWiseEmployeeColumns>>(true, "Records found", groupedData.ToList(), 200);
        }
        private void AddMasterSheet(ExcelPackage package, string sheetName, string[] headers, IEnumerable<dynamic> data)
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            int rowIndex = 2;
            foreach (var row in data)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[rowIndex, i + 1].Value = ((IDictionary<string, object>)row)[headers[i]]?.ToString();
                }
                rowIndex++;
            }
        }
        public async Task<ServiceResponse<List<EmployeeDocument>>> GetEmployeeDocuments(int employee_id)
        {
            try
            {
                var data = await _connection.QueryAsync<EmployeeDocument>(
                   "SELECT * FROM tbl_DocumentsMaster WHERE employee_id = @employee_id",
                   new { employee_id }) ?? throw new Exception("Data not found");
                string filePath = string.Empty;

                foreach (var item in data)
                {
                    item.file_path = GetImage(item.file_path);
                }
                return new ServiceResponse<List<EmployeeDocument>>(true, "Record Found", data.AsList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeDocument>>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<List<EmployeeQualification>>> GetEmployeeQualificationById(int employeeId)
        {
            try
            {
                string query = @"
                SELECT 
                    [Qualification_Info_id],
                    [employee_id],
                    [Educational_Qualification],
                    [Year_of_Completion]
                FROM 
                    [tbl_QualificationInfoMaster]
                WHERE 
                    [employee_id] = @EmployeeId";

                var data = await _connection.QueryAsync<EmployeeQualification>(query, new { EmployeeId = employeeId });
                if (data != null)
                {
                    return new ServiceResponse<List<EmployeeQualification>>(true, "Records found", data.AsList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeQualification>>(false, "no Records found", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeQualification>>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<List<EmployeeWorkExperience>>> GetEmployeeWorkExperienceById(int employeeId)
        {
            try
            {
                string query = @"
                SELECT 
                    [work_experience_id],
                    [Year],
                    [Month],
                    [Previous_Organisation],
                    [Previous_Designation],
                    [Employee_id]
                FROM 
                    [tbl_WorkExperienceMaster]
                WHERE 
                    [Employee_id] = @EmployeeId";

                var data = await _connection.QueryAsync<EmployeeWorkExperience>(query, new { EmployeeId = employeeId });
                if (data != null)
                {
                    return new ServiceResponse<List<EmployeeWorkExperience>>(true, "Records found", data.AsList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeWorkExperience>>(false, "no Records found", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeWorkExperience>>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<List<EmployeeBankDetails>>> GetEmployeeBankDetailsById(int employeeId)
        {
            try
            {
                string query = @"
                SELECT 
                    [bank_id],
                    [employee_id],
                    [bank_name],
                    [account_name],
                    [account_number],
                    [IFSC_code],
                    [Bank_address]
                FROM 
                    [tbl_BankDetailsmaster]
                WHERE 
                    [employee_id] = @EmployeeId";

                var data = await _connection.QueryAsync<EmployeeBankDetails>(query, new { EmployeeId = employeeId });
                if (data != null)
                {
                    return new ServiceResponse<List<EmployeeBankDetails>>(true, "Records found", data.AsList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeBankDetails>>(false, "no Records found", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeBankDetails>>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<EmployeeAddressResponse>> GetEmployeeAddressDetailsById(int employeeId)
        {
            try
            {
                // SQL query to retrieve employee address details along with country, state, city, and district names
                string sql = @"
        SELECT 
            e.Employee_Present_Address_id,
            e.Address,
            e.CountryName,
            e.StateName,
            e.CityName,
            e.DistrictName,
            e.Pin_code,
            e.AddressTypeId,
            e.Employee_id
        FROM tbl_EmployeePresentAddress e
        WHERE e.Employee_id = @EmployeeId"; // Assuming IsDeleted is a column to check if the address is active

                // Execute the query and retrieve the address details
                var addressDetails = await _connection.QueryAsync<EmployeeAddressDetailsResponse>(sql, new { EmployeeId = employeeId });

                if (addressDetails == null || !addressDetails.Any())
                {
                    return new ServiceResponse<EmployeeAddressResponse>(false, "No address details found for the given employee.", null, 204);
                }

                // Create response object to hold present and permanent addresses
                var response = new EmployeeAddressResponse
                {
                    PresentAddress = addressDetails
                        .Where(a => a.AddressTypeId == 2) // Present addresses
                        .ToList(),
                    PermanentAddress = addressDetails
                        .Where(a => a.AddressTypeId == 1) // Permanent addresses
                        .ToList()
                };

                return new ServiceResponse<EmployeeAddressResponse>(true, "Address details retrieved successfully.", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeAddressResponse>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<EmployeeStaffMappingResponse>> GetEmployeeMappingById(int employeeId)
        {
            var response = new EmployeeStaffMappingResponse { EmployeeId = employeeId };

            // Query for EmployeeStaffMapClassTeacher
            string teacherMappingsSql = @"
    SELECT t.MappingId, 
           t.EmployeeId, 
           t.ClassId, 
           c.class_name, 
           t.SectionId, 
           s.section_name, 
           sub.SubjectId, 
           sub.SubjectName
    FROM tbl_EmployeeStaffMapClassTeacher t
    INNER JOIN tbl_Class c ON t.ClassId = c.class_id
    LEFT JOIN tbl_Section s ON t.SectionId = s.section_id AND s.IsDeleted = 0
    LEFT JOIN tbl_Subjects sub ON sub.SubjectId IN (
        SELECT value FROM STRING_SPLIT(t.SubjectId, ',')
    ) AND sub.IsDeleted = 0
    WHERE t.EmployeeId = @EmployeeId";

            var teacherMappings = await _connection.QueryAsync<dynamic>(teacherMappingsSql, new { EmployeeId = employeeId });

            // Group and map the teacher mappings by class and section
            var groupedTeacherMappings = teacherMappings
                .GroupBy(m => new { m.ClassId, m.SectionId })
                .Select(g => new EmployeeStaffMapClassTeacherResponse
                {
                    MappingId = g.First().MappingId,
                    ClassId = g.First().ClassId,
                    ClassName = g.First().class_name,
                    SectionId = g.First().SectionId,
                    SectionName = g.First().section_name,
                    subjects = g.Select(s => new Subjects
                    {
                        SubjectId = s.SubjectId,
                        SubjectName = s.SubjectName
                    }).ToList()
                }).ToList(); // Return the entire list

            response.EmployeeStaffMappingsClassTeacher = groupedTeacherMappings;

            // Query for EmployeeStappMapClassSection
            string sectionMappingsSql = @"
SELECT e.ClassSectionMapId, 
       e.EmployeeId, 
       e.SubjectId, 
       sub.SubjectName, 
       e.ClassId, 
       c.class_name, 
       s.section_id, 
       s.section_name
FROM tbl_EmployeeStappMapClassSection e
INNER JOIN tbl_Class c ON e.ClassId = c.class_id
LEFT JOIN tbl_Section s 
    ON s.section_id IN (
        SELECT value FROM STRING_SPLIT(e.SectionId, ',')
    ) AND s.IsDeleted = 0
LEFT JOIN tbl_Subjects sub 
    ON e.SubjectId = sub.SubjectId
WHERE e.EmployeeId = @EmployeeId";

            var sectionMappings = await _connection.QueryAsync<dynamic>(sectionMappingsSql, new { EmployeeId = employeeId });

            // Group and map the section mappings by class and sections
            var groupedSectionMappings = sectionMappings
                .GroupBy(m => new { m.ClassId, m.ClassSectionMapId }) // Group by class ID and ClassSectionMapId
                .Select(g => new EmployeeStappMapClassSectionResponse
                {
                    ClassSectionMapId = g.Key.ClassSectionMapId,
                    ClassId = g.Key.ClassId,
                    ClassName = g.First().class_name,
                    SubjectId = g.First().SubjectId,
                    SubjectName = g.First().SubjectName,
                    sections = g.GroupBy(sec => sec.section_id)
                                .Select(secGroup => new Sections
                                {
                                    SectionId = secGroup.Key,
                                    SectionName = secGroup.First().section_name
                                }).ToList()
                }).ToList();

            response.EmployeeStappMappingsClassSection = groupedSectionMappings;

            return new ServiceResponse<EmployeeStaffMappingResponse>(true, "Records found", response, 200);
        }
        public async Task<ServiceResponse<List<ClassSectionList>>> GetClassSectionList(int instituteId)
        {
            // Define the SQL query to join classes and sections
            var query = @"
        SELECT 
            c.class_id AS ClassId,
            s.section_id AS SectionId,
            CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection
        FROM 
            tbl_Class c
        JOIN 
            tbl_Section s ON c.class_id = s.class_id
        WHERE 
            c.institute_id = @InstituteId AND c.IsDeleted = 0 AND s.IsDeleted = 0
        ORDER BY 
            c.class_name, s.section_name;";

            try
            {

                // Open the connection
                _connection.Open();

                // Execute the query and map results
                var classSectionList = await _connection.QueryAsync<ClassSectionList>(query, new { InstituteId = instituteId });
                return new ServiceResponse<List<ClassSectionList>>(true, "Class section list retrieved successfully.", classSectionList.ToList(), 200);

            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClassSectionList>>(false, ex.Message, [], 500);
            }
            finally
            {
                _connection.Close();
            }
        }
        public async Task<ServiceResponse<ClassSectionSubjectResponse>> ClassSectionSubjectsList(int classId, int sectionId)
        {

            // Define the SQL query to get subjects based on classId and sectionId
            var query = @"
            SELECT 
            csm.class_id AS ClassId,
            csm.section_id AS SectionId,
            s.SubjectId,
            s.SubjectName
        FROM 
            tbl_ClassSectionSubjectMapping csm
        JOIN 
            tbl_Subjects s ON csm.SubjectId = s.SubjectId
        WHERE 
            csm.class_id = @ClassId AND
            ',' + csm.section_id + ',' LIKE '%,' + CAST(@SectionIdFilter AS VARCHAR) + ',%' AND
            csm.IsDeleted = 0 AND
            s.IsDeleted = 0;";

            try
            {
                // Execute the query and fetch results
                var subjectList = await _connection.QueryAsync<Subjects>(query, new
                {
                    ClassId = classId,
                    SectionIdFilter = sectionId // Format for CSV matching
                });

                // Check if any subjects were found
                if (subjectList.Any())
                {
                    // Create the response object with subjects
                    var classSectionSubjectResponse = new ClassSectionSubjectResponse
                    {
                        classId = classId,
                        SectionId = sectionId,
                        subjects = subjectList.Select(s => new Subjects
                        {
                            SubjectId = s.SubjectId,
                            SubjectName = s.SubjectName
                        }).ToList()
                    };
                    return new ServiceResponse<ClassSectionSubjectResponse>(true, "Subjects retrieved successfully.", classSectionSubjectResponse, 200);
                }
                else
                {
                    return new ServiceResponse<ClassSectionSubjectResponse>(false, "No subjects found for the specified class and section.", new ClassSectionSubjectResponse(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ClassSectionSubjectResponse>(false, ex.Message, new ClassSectionSubjectResponse(), 500);
            }
        }
        public async Task<ServiceResponse<List<ClassSectionSubjectResponse>>> ClassSectionSubjectsMappings(int InstituteId)
        {
            // Adjusted SQL query to fetch class and section names
            var query = @"
SELECT 
    csm.class_id AS ClassId,
    csm.section_id AS SectionIdCSV, -- section_id is stored as a CSV
    c.class_name,
    s.SubjectId,
    s.SubjectName,
    sec.section_id,
    sec.section_name
FROM 
    tbl_ClassSectionSubjectMapping csm
JOIN 
    tbl_Subjects s ON csm.SubjectId = s.SubjectId
JOIN 
    tbl_Class c ON csm.class_id = c.class_id
LEFT JOIN 
    tbl_Section sec ON CHARINDEX(',' + CAST(sec.section_id AS VARCHAR) + ',', ',' + csm.section_id + ',') > 0
WHERE 
    c.institute_id = @InstituteId AND
    csm.IsDeleted = 0 AND
    s.IsDeleted = 0;";

            try
            {
                // Execute the query and fetch results
                var mappingsList = await _connection.QueryAsync<dynamic>(query, new { InstituteId });

                // Check if any mappings were found
                if (mappingsList.Any())
                {
                    // Process section IDs stored as CSV strings
                    var processedMappings = mappingsList.SelectMany(m =>
                    {
                        // Split the CSV string of section IDs and convert to integers
                        var sectionIds = ((string)m.SectionIdCSV).Split(',')
                                         .Where(id => int.TryParse(id, out _))  // Filter valid integers
                                         .Select(int.Parse).ToList();           // Convert valid strings to integers

                        // Create separate mappings for each section ID
                        return sectionIds.Select(sectionId => new ClassSectionSubjectResponse
                        {
                            classId = (int)m.ClassId,
                            ClassName = (string)m.class_name,  // Map class name
                            SectionId = sectionId,
                            SectionName = (string)m.section_name, // Map section name
                            subjects = new List<Subjects>
        {
            new Subjects
            {
                SubjectId = (int)m.SubjectId,
                SubjectName = (string)m.SubjectName
            }
        }
                        });
                    }).ToList();


                    // Check if we have valid mappings
                    if (processedMappings.Any())
                    {
                        return new ServiceResponse<List<ClassSectionSubjectResponse>>(true, "Subject mappings retrieved successfully.", processedMappings, 200);
                    }
                    else
                    {
                        return new ServiceResponse<List<ClassSectionSubjectResponse>>(false, "No valid section IDs found.", new List<ClassSectionSubjectResponse>(), 404);
                    }
                }
                else
                {
                    return new ServiceResponse<List<ClassSectionSubjectResponse>>(false, "No subject mappings found for the specified institute.", new List<ClassSectionSubjectResponse>(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClassSectionSubjectResponse>>(false, $"An error occurred: {ex.Message}", new List<ClassSectionSubjectResponse>(), 500);
            }
        }
        public async Task<ServiceResponse<bool>> StatusActiveInactive(EmployeeStatusRequest request)
        {
            try
            {
                var data = await GetEmployeeProfileById(request.EmployeeId);

                if (data.Data != null)
                {
                    bool Status = !data.Data.Status;

                    string sql = "UPDATE tbl_EmployeeProfileMaster SET Status = @Status, InActiveReason = @InActiveReason WHERE Employee_id = @Employee_id";

                    int rowsAffected = await _connection.ExecuteAsync(sql, new { Status, Employee_id = request.EmployeeId, InActiveReason = request.InActiveReason });
                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<bool>(true, "Operation Successful", true, 200);
                    }
                    else
                    {
                        return new ServiceResponse<bool>(false, "Opertion Failed", false, 500);
                    }
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Record not Found", false, 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<List<MaritalStatus>>> GetMaritalStatusList()
        {
            try
            {
                string sql = @"
        SELECT statusId, statusName 
        FROM [tbl_MaritalStatus]";

                var data = await _connection.QueryAsync<MaritalStatus>(sql);

                if (data != null && data.Any())
                {
                    return new ServiceResponse<List<MaritalStatus>>(true, "Marital status list retrieved successfully", data.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<MaritalStatus>>(false, "No marital statuses found", new List<MaritalStatus>(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<MaritalStatus>>(false, ex.Message, new List<MaritalStatus>(), 500);
            }
        }
        public async Task<ServiceResponse<List<BloodGroup>>> GetBloodGroupList()
        {
            try
            {
                string sql = @"
        SELECT [Blood_Group_id], [Blood_Group_Type]
        FROM [tbl_BloodGroup]";

                var data = await _connection.QueryAsync<BloodGroup>(sql);

                if (data != null && data.Any())
                {
                    return new ServiceResponse<List<BloodGroup>>(true, "Blood group list retrieved successfully", data.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<BloodGroup>>(false, "No blood groups found", new List<BloodGroup>(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<BloodGroup>>(false, ex.Message, new List<BloodGroup>(), 500);
            }
        }
        public async Task<ServiceResponse<List<Designation>>> GetDesignationList(int DepartmentId)
        {
            try
            {
                string sql = @"
        SELECT [Designation_id], 
               [Institute_id], 
               [DesignationName], 
               [Department_id], 
               [IsDeleted]
        FROM [tbl_Designation]
        WHERE Department_id = @DepartmentId AND IsDeleted = 0"; // Only fetch non-deleted designations

                var parameters = new { DepartmentId };

                var data = await _connection.QueryAsync<Designation>(sql, parameters);

                if (data != null && data.Any())
                {
                    return new ServiceResponse<List<Designation>>(true, "Designation list retrieved successfully", data.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<Designation>>(false, "No designations found for the specified department", new List<Designation>(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Designation>>(false, ex.Message, new List<Designation>(), 500);
            }
        }
        public async Task<ServiceResponse<List<Department>>> GetDepartmentList(int InstituteId)
        {
            try
            {
                string sql = @"
        SELECT [Department_id], 
               [Institute_id], 
               [DepartmentName], 
               [IsDeleted]
        FROM [tbl_Department]
        WHERE Institute_id = @InstituteId AND IsDeleted = 0"; // Only fetch non-deleted departments

                var parameters = new { InstituteId };

                var data = await _connection.QueryAsync<Department>(sql, parameters);

                if (data != null && data.Any())
                {
                    return new ServiceResponse<List<Department>>(true, "Department list retrieved successfully", data.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<Department>>(false, "No departments found for the specified institute", new List<Department>(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Department>>(false, ex.Message, new List<Department>(), 500);
            }
        }
        public async Task<ServiceResponse<bool>> UpdatePassword(ChangePasswordRequest request)
        {
            try
            {
                // Ensure the new password is at least 8 characters
                if (request.NewPassword.Length < 8)
                {
                    return new ServiceResponse<bool>(false, "New password must be at least 8 characters", false, 400);
                }

                // Query to validate the username and current password
                string validateUserSql = @"
        SELECT COUNT(1) 
        FROM tblLoginInformationMaster
        WHERE UserName = @UserName AND Password = @OldPassword";

                // Check if the user exists with the current password
                int userExists = await _connection.ExecuteScalarAsync<int>(validateUserSql, new
                {
                    UserName = request.Username,   // Corrected to UserName column
                    OldPassword = request.OldPassword
                });

                // If no user found with the given username and current password
                if (userExists == 0)
                {
                    return new ServiceResponse<bool>(false, "Invalid username or current password", false, 401);
                }

                // SQL query to update the password
                string updatePasswordSql = @"
        UPDATE tblLoginInformationMaster
        SET Password = @NewPassword, UserActivity = @ActivityDescription
        WHERE UserName = @UserName";

                // Update the password in the database
                int rowsAffected = await _connection.ExecuteAsync(updatePasswordSql, new
                {
                    UserName = request.Username,    // Corrected to UserName column
                    NewPassword = request.NewPassword,
                    ActivityDescription = DateTime.Now
                });

                // Return true if the password was successfully updated
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Password updated successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Password update failed", false, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        //public async Task<ServiceResponse<byte[]>> ExcelDownload(ExcelDownloadRequest request)
        //{
        //    try
        //    {
        //        // Define your query with filters for DesignationId and DepartmentId
        //        string query = @"SELECT 
        //                    Employee_id AS EmployeeId,
        //                    CONCAT(First_Name, ' ', Middle_Name, ' ', Last_Name) AS EmployeeName,
        //                    d.DepartmentName AS Department,
        //                    des.DesignationName AS Designation,
        //                    g.Gender_Type AS Gender,
        //                    mobile_number AS Mobile,
        //                    Date_of_Birth AS DateOfBirth,
        //                    EmailID AS Email
        //                 FROM tbl_EmployeeProfileMaster e
        //                 LEFT JOIN tbl_Department d ON e.Department_id = d.Department_id
        //                 LEFT JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        //                 LEFT JOIN tbl_Gender g ON e.Gender_id = g.Gender_id
        //                 WHERE e.Institute_id = @InstituteId
        //                 AND (@DesignationId IS NULL OR e.Designation_id = @DesignationId)
        //                 AND (@DepartmentId IS NULL OR e.Department_id = @DepartmentId)
        //                 AND e.Status = 1";

        //        // Execute the query with filters
        //        var employeeProfiles = (await _connection.QueryAsync<dynamic>(
        //            query,
        //            new
        //            {
        //                request.InstituteId,
        //                DesignationId = request.DesignationId == 0 ? (int?)null : request.DesignationId,
        //                DepartmentId = request.DepartmetnId == 0 ? (int?)null : request.DepartmetnId
        //            })).ToList();

        //        // If no records found, create Excel file with only headers
        //        if (!employeeProfiles.Any())
        //        {
        //            using (var package = new ExcelPackage())
        //            {
        //                var worksheet = package.Workbook.Worksheets.Add("Employee Data");

        //                // Add headers
        //                worksheet.Cells[1, 1].Value = "Employee ID";
        //                worksheet.Cells[1, 2].Value = "Employee Name";
        //                worksheet.Cells[1, 3].Value = "Department";
        //                worksheet.Cells[1, 4].Value = "Designation";
        //                worksheet.Cells[1, 5].Value = "Gender";
        //                worksheet.Cells[1, 6].Value = "Mobile";
        //                worksheet.Cells[1, 7].Value = "Date of Birth";
        //                worksheet.Cells[1, 8].Value = "Email";

        //                // Convert to byte array
        //                var stream = new MemoryStream();
        //                package.SaveAs(stream);
        //                var fileData = stream.ToArray();
        //                return new ServiceResponse<byte[]>(true, "No records found, only headers included.", fileData, StatusCodes.Status200OK);
        //            }
        //        }

        //        // Create Excel file with data
        //        using (var package = new ExcelPackage())
        //        {
        //            var worksheet = package.Workbook.Worksheets.Add("Employee Data");

        //            // Add headers
        //            worksheet.Cells[1, 1].Value = "Employee ID";
        //            worksheet.Cells[1, 2].Value = "Employee Name";
        //            worksheet.Cells[1, 3].Value = "Department";
        //            worksheet.Cells[1, 4].Value = "Designation";
        //            worksheet.Cells[1, 5].Value = "Gender";
        //            worksheet.Cells[1, 6].Value = "Mobile";
        //            worksheet.Cells[1, 7].Value = "Date of Birth";
        //            worksheet.Cells[1, 8].Value = "Email";

        //            // Add data rows
        //            for (int i = 0; i < employeeProfiles.Count; i++)
        //            {
        //                var profile = employeeProfiles[i];
        //                worksheet.Cells[i + 2, 1].Value = profile.EmployeeId;
        //                worksheet.Cells[i + 2, 2].Value = profile.EmployeeName;
        //                worksheet.Cells[i + 2, 3].Value = profile.Department;
        //                worksheet.Cells[i + 2, 4].Value = profile.Designation;
        //                worksheet.Cells[i + 2, 5].Value = profile.Gender;
        //                worksheet.Cells[i + 2, 6].Value = profile.Mobile;
        //                worksheet.Cells[i + 2, 7].Value = profile.DateOfBirth;
        //                worksheet.Cells[i + 2, 8].Value = profile.Email;
        //            }

        //            // Format the header cells
        //            using (var range = worksheet.Cells[1, 1, 1, 8])
        //            {
        //                range.Style.Font.Bold = true;
        //                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        //                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            }

        //            // Convert to byte array
        //            var stream = new MemoryStream();
        //            package.SaveAs(stream);
        //            var fileData = stream.ToArray();
        //            return new ServiceResponse<byte[]>(true, "Excel generated successfully.", fileData, StatusCodes.Status200OK);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<byte[]>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
        //    }
        //}
        public async Task<ServiceResponse<byte[]>> ExcelDownload(ExcelDownloadRequest request, string format)
        {
            try
            {
                // Define your query with filters for DesignationId and DepartmentId
                string query = @"SELECT 
                            Employee_id AS EmployeeId,
                            CONCAT(First_Name, ' ', Middle_Name, ' ', Last_Name) AS EmployeeName,
                            d.DepartmentName AS Department,
                            des.DesignationName AS Designation,
                            g.Gender_Type AS Gender,
                            mobile_number AS Mobile,
                            Date_of_Birth AS DateOfBirth,
                            EmailID AS Email
                        FROM tbl_EmployeeProfileMaster e
                        LEFT JOIN tbl_Department d ON e.Department_id = d.Department_id
                        LEFT JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
                        LEFT JOIN tbl_Gender g ON e.Gender_id = g.Gender_id
                        WHERE e.Institute_id = @InstituteId
                        AND (@DesignationId IS NULL OR e.Designation_id = @DesignationId)
                        AND (@DepartmentId IS NULL OR e.Department_id = @DepartmentId)
                        AND e.Status = 1";

                // Execute the query with filters
                var employeeProfiles = (await _connection.QueryAsync<dynamic>(
                    query,
                    new
                    {
                        request.InstituteId,
                        DesignationId = request.DesignationId == 0 ? (int?)null : request.DesignationId,
                        DepartmentId = request.DepartmetnId == 0 ? (int?)null : request.DepartmetnId
                    })).ToList();

                var ipAddress = GetClientIPAddress();
                string historyQuery = @"insert into tbl_EmployeeExportHistory ( EmployeeCount ,DownloadDate ,IPAddress,Username, InstituteId)
                                           VALUES 
                        ( @EmployeeCount ,@DownloadDate ,@IPAddress,@Username, @InstituteId)";
                await _connection.ExecuteAsync(historyQuery, new
                {
                    EmployeeCount = employeeProfiles.Count,
                    DownloadDate = DateTime.Now,
                    IPAddress = ipAddress,
                    Username = "", // Assuming you have this in the request
                    InstituteId = request.InstituteId
                });
                if (!employeeProfiles.Any())
                {
                    return await GenerateFileWithoutData(format);
                }

                if (format.ToLower() == "csv")
                {
                    return GenerateCSVFile(employeeProfiles);
                }
                else
                {
                    return GenerateExcelFile(employeeProfiles);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
            }
        }
        private string GetClientIPAddress()
        {
            // Getting the forwarded IP address from headers (if behind a proxy or load balancer)
            var ipAddress = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            // If no forwarded IP address is found, use the remote IP address
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            }

            return ipAddress ?? "Unknown"; // Return "Unknown" if the IP address could not be retrieved
        }
        private async Task<ServiceResponse<byte[]>> GenerateFileWithoutData(string format)
        {
            if (format.ToLower() == "csv")
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("Employee ID,Employee Name,Department,Designation,Gender,Mobile,Date of Birth,Email");

                var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                return new ServiceResponse<byte[]>(true, "No records found, only headers included.", csvBytes, StatusCodes.Status200OK);
            }
            else
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Employee Data");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Employee ID";
                    worksheet.Cells[1, 2].Value = "Employee Name";
                    worksheet.Cells[1, 3].Value = "Department";
                    worksheet.Cells[1, 4].Value = "Designation";
                    worksheet.Cells[1, 5].Value = "Gender";
                    worksheet.Cells[1, 6].Value = "Mobile";
                    worksheet.Cells[1, 7].Value = "Date of Birth";
                    worksheet.Cells[1, 8].Value = "Email";

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    var fileData = stream.ToArray();
                    return new ServiceResponse<byte[]>(true, "No records found, only headers included.", fileData, StatusCodes.Status200OK);
                }
            }
        }
        private ServiceResponse<byte[]> GenerateCSVFile(List<dynamic> employeeProfiles)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Employee ID,Employee Name,Department,Designation,Gender,Mobile,Date of Birth,Email");

            foreach (var profile in employeeProfiles)
            {
                csvBuilder.AppendLine($"{profile.EmployeeId},{profile.EmployeeName},{profile.Department},{profile.Designation},{profile.Gender},{profile.Mobile},{profile.DateOfBirth},{profile.Email}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return new ServiceResponse<byte[]>(true, "CSV file generated successfully.", csvBytes, StatusCodes.Status200OK);
        }
        private ServiceResponse<byte[]> GenerateExcelFile(List<dynamic> employeeProfiles)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employee Data");

                // Add headers
                worksheet.Cells[1, 1].Value = "Employee ID";
                worksheet.Cells[1, 2].Value = "Employee Name";
                worksheet.Cells[1, 3].Value = "Department";
                worksheet.Cells[1, 4].Value = "Designation";
                worksheet.Cells[1, 5].Value = "Gender";
                worksheet.Cells[1, 6].Value = "Mobile";
                worksheet.Cells[1, 7].Value = "Date of Birth";
                worksheet.Cells[1, 8].Value = "Email";

                // Add data rows
                for (int i = 0; i < employeeProfiles.Count; i++)
                {
                    var profile = employeeProfiles[i];
                    worksheet.Cells[i + 2, 1].Value = profile.EmployeeId;
                    worksheet.Cells[i + 2, 2].Value = profile.EmployeeName;
                    worksheet.Cells[i + 2, 3].Value = profile.Department;
                    worksheet.Cells[i + 2, 4].Value = profile.Designation;
                    worksheet.Cells[i + 2, 5].Value = profile.Gender;
                    worksheet.Cells[i + 2, 6].Value = profile.Mobile;
                    worksheet.Cells[i + 2, 7].Value = profile.DateOfBirth;
                    worksheet.Cells[i + 2, 8].Value = profile.Email;
                }

                // Format the header cells
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                var fileData = stream.ToArray();
                return new ServiceResponse<byte[]>(true, "Excel file generated successfully.", fileData, StatusCodes.Status200OK);
            }
        }
        private async Task<bool> CreateUserLoginInfo(int userId, int userType, int instituteId)
        {
            try
            {
                var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(); // Ensure async method for opening

                // Define common password
                string commonPassword = "iGuru@1234";

                // SQL queries for fetching user details based on UserType
                string employeeSql = @"
            SELECT TOP (1) [Employee_id], [First_Name], [Last_Name]
            FROM [tbl_EmployeeProfileMaster]
            WHERE [Employee_id] = @UserId";

                string studentSql = @"
            SELECT TOP (1) [student_id], [First_Name], [Last_Name]
            FROM [tbl_StudentMaster]
            WHERE [student_id] = @UserId";

                // Initialize variables
                string username = null;
                dynamic userDetails = null;
                string institutesql = @"select Institute_name from tbl_InstituteDetails where Institute_id = @Institute_id;";
                var instituteName = await _connection.QueryFirstOrDefaultAsync<string>(institutesql, new { Institute_id = instituteId });

                // Fetch user details based on the UserType
                if (userType == 1) // Employee
                {
                    userDetails = await connection.QueryFirstOrDefaultAsync<dynamic>(employeeSql, new { UserId = userId });
                    if (userDetails != null)
                    {
                        // Generate username for employee
                        username = await GenerateUsername(instituteName, instituteId, "E", userId);
                    }
                }
                else if (userType == 2) // Student
                {
                    userDetails = await connection.QueryFirstOrDefaultAsync<dynamic>(studentSql, new { UserId = userId });
                    if (userDetails != null)
                    {
                        // Generate username for student
                        username = await GenerateUsername(instituteName, instituteId, "S", userId);
                    }
                }

                if (username != null)
                {
                    // Ensure the username is unique
                    username = await EnsureUniqueUsername(username);

                    // SQL query to insert login information
                    string insertLoginSql = @"
                INSERT INTO [tblLoginInformationMaster] 
                ([UserId], [UserType], [UserName], [Password], [InstituteId], [UserActivity])
                VALUES (@UserId, @UserType, @UserName, @Password, @InstituteId, NULL)";

                    // Insert login information into the database
                    await connection.ExecuteAsync(insertLoginSql, new
                    {
                        UserId = userId,
                        UserType = userType,
                        UserName = username,
                        Password = commonPassword,
                        InstituteId = instituteId
                    });

                    return true; // Operation successful
                }

                return false; // User details not found or unable to create login info
            }
            catch (Exception ex)
            {
                // Log the exception and return false to indicate failure
                Console.WriteLine($"Error creating user login info: {ex.Message}");
                return false;
            }
        }
        private async Task<string> GenerateUsername(string instituteName, int instituteId, string roleIdentifier, int userId)
        {
            // Step 1: Get the first four letters of the institute name
            string institutePrefix = GetInstitutePrefix(instituteName);

            // Step 2: Concatenate the Institute ID and Role Identifier
            string baseUsername = $"{institutePrefix}{instituteId}{roleIdentifier}";

            // Step 3: Append the sequence number (dynamic based on user type and role)
            int sequenceNumber = await GetNextSequenceNumber(instituteId, roleIdentifier);

            // Return the generated username
            return $"{baseUsername}{sequenceNumber}";
        }
        private string GetInstitutePrefix(string instituteName)
        {
            // Logic to extract the first four meaningful letters from the institute name
            var words = instituteName.Split(' ');
            string prefix = string.Empty;

            foreach (var word in words)
            {
                if (!string.IsNullOrEmpty(word) && prefix.Length < 4)
                {
                    prefix += word[0].ToString().ToUpper();
                }
            }

            // Ensure the prefix is exactly 4 characters
            return prefix.PadRight(4, 'X').Substring(0, 4);
        }
        private async Task<int> GetNextSequenceNumber(int instituteId, string roleIdentifier)
        {
            var connection = new SqlConnection(_connectionString);

            // SQL query to get the current max sequence number for the given institute and role
            string sequenceSql = @"
        SELECT ISNULL(MAX(CAST(SUBSTRING(UserName, LEN(UserName) - LEN(@RoleIdentifier) + 1, LEN(@RoleIdentifier)) AS INT)), 0)
        FROM [tblLoginInformationMaster]
        WHERE InstituteId = @InstituteId AND UserName LIKE @Prefix + '%'";

            string prefix = $"{roleIdentifier}{instituteId}";

            int currentMaxSequence = await connection.ExecuteScalarAsync<int>(sequenceSql, new
            {
                InstituteId = instituteId,
                RoleIdentifier = roleIdentifier,
                Prefix = prefix
            });

            return currentMaxSequence + 1; // Return the next sequence number
        }

        //    private async Task<bool> CreateUserLoginInfo(int userId, int userType, int instituteId)
        //    {
        //        try
        //        {
        //            var connection = new SqlConnection(_connectionString);
        //            await connection.OpenAsync(); // Ensure async method for opening

        //            // Define common password
        //            string commonPassword = "iGuru@1234";

        //            // SQL queries for fetching user details based on UserType
        //            string employeeSql = @"
        //        SELECT TOP (1) [Employee_id], [First_Name], [Last_Name], [mobile_number]
        //        FROM [tbl_EmployeeProfileMaster]
        //        WHERE [Employee_id] = @UserId";

        //            string studentSql = @"
        //        SELECT TOP (1) [student_id], [First_Name], [Last_Name], [Admission_Number]
        //        FROM [tbl_StudentMaster]
        //        WHERE [student_id] = @UserId";

        //            // Initialize variables
        //            string username = null;
        //            dynamic userDetails = null;

        //            // Fetch user details based on the UserType
        //            if (userType == 1) // Employee
        //            {
        //                userDetails = await connection.QueryFirstOrDefaultAsync<dynamic>(employeeSql, new { UserId = userId });
        //                if (userDetails != null)
        //                {
        //                    // Construct username for employee
        //                    string firstName = userDetails.First_Name;
        //                    string lastName = userDetails.Last_Name;
        //                    string phoneNumber = userDetails.mobile_number;

        //                    // Ensure the first name and last name have at least 3 characters and the phone number has at least 4 characters
        //                    if (firstName.Length >= 3 && lastName.Length >= 3 && phoneNumber.Length >= 4)
        //                    {
        //                        username = $"{firstName.Substring(0, 3)}{lastName.Substring(0, 3)}{phoneNumber.Substring(phoneNumber.Length - 4)}";
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("First name, last name, or phone number too short for username creation.");
        //                    }
        //                }
        //            }
        //            else if (userType == 2) // Student
        //            {
        //                userDetails = await connection.QueryFirstOrDefaultAsync<dynamic>(studentSql, new { UserId = userId });
        //                if (userDetails != null)
        //                {
        //                    // Construct username for student
        //                    string firstName = userDetails.First_Name;
        //                    string lastName = userDetails.Last_Name;
        //                    string admissionNumber = userDetails.Admission_Number;

        //                    // Ensure the first name and last name have at least 3 characters and the admission number has at least 4 characters
        //                    if (firstName.Length >= 3 && lastName.Length >= 3 && admissionNumber.Length >= 4)
        //                    {
        //                        username = $"{firstName.Substring(0, 3)}{lastName.Substring(0, 3)}{admissionNumber.Substring(admissionNumber.Length - 4)}";
        //                    }
        //                    else
        //                    {
        //                        throw new Exception("First name, last name, or admission number too short for username creation.");
        //                    }
        //                }
        //            }

        //            if (username != null)
        //            {
        //                // Ensure the username is unique
        //                username = await EnsureUniqueUsername(username);

        //                // SQL query to insert login information
        //                string insertLoginSql = @"
        //            INSERT INTO [tblLoginInformationMaster] 
        //            ([UserId], [UserType], [UserName], [Password], [InstituteId], [UserActivity])
        //            VALUES (@UserId, @UserType, @UserName, @Password, @InstituteId, NULL)";

        //                // Insert login information into the database
        //                await connection.ExecuteAsync(insertLoginSql, new
        //                {
        //                    UserId = userId,
        //                    UserType = userType,
        //                    UserName = username,
        //                    Password = commonPassword,
        //                    InstituteId = instituteId
        //                });

        //                return true; // Operation successful
        //            }

        //            return false; // User details not found or unable to create login info
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log the exception and return false to indicate failure
        //            Console.WriteLine($"Error creating user login info: {ex.Message}");
        //            return false;
        //        }
        //    }
        private async Task<string> EnsureUniqueUsername(string baseUsername)
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            // Define the SQL query to check if the username exists
            string checkUsernameSql = @"
        SELECT COUNT(1)
        FROM [tblLoginInformationMaster]
        WHERE [UserName] = @UserName";

            string uniqueUsername = baseUsername;
            int suffix = 1;

            // Check if the username already exists
            while (await connection.ExecuteScalarAsync<int>(checkUsernameSql, new { UserName = uniqueUsername }) > 0)
            {
                // Append a numeric suffix to make the username unique
                uniqueUsername = $"{baseUsername}{suffix}";
                suffix++;
            }

            return uniqueUsername; // Return the unique username
        }
        private string ImageUpload(string image)
        {
            if (string.IsNullOrEmpty(image) || image == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "EmployeeProfile");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileExtension = IsJpeg(imageData) == true ? ".jpg" : IsPng(imageData) == true ? ".png" : IsGif(imageData) == true ? ".gif" : string.Empty;
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(directoryPath, fileName);
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new InvalidOperationException("Incorrect file uploaded");
            }
            // Write the byte array to the image file
            File.WriteAllBytes(filePath, imageData);
            return filePath;
        }
        private bool IsJpeg(byte[] bytes)
        {
            // JPEG magic number: 0xFF, 0xD8
            return bytes.Length > 1 && bytes[0] == 0xFF && bytes[1] == 0xD8;
        }
        private bool IsPng(byte[] bytes)
        {
            // PNG magic number: 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
            return bytes.Length > 7 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47
                && bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;
        }
        private bool IsGif(byte[] bytes)
        {
            // GIF magic number: "GIF"
            return bytes.Length > 2 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46;
        }
        private string GetImage(string Filename)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "EmployeeProfile", Filename);

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        public async Task<List<ColumnDetails>> GetActiveColumns(List<ActiveColumns>? activeColumns)
        {
            var activeColumnDetails = new List<ColumnDetails>();
            if (activeColumns != null)
            {
                foreach (var columnRequest in activeColumns)
                {
                    var updateQuery = @"
        UPDATE tbl_EmployeeColumnMaster
        SET Status = @Status
        WHERE ECMId = @ECMId
        AND CategoryId = @CategoryId";

                    var updateParameters = new DynamicParameters();
                    updateParameters.Add("Status", columnRequest.Status); // Set status based on request
                    updateParameters.Add("ECMId", columnRequest.ECMId);
                    updateParameters.Add("CategoryId", columnRequest.CategoryId);

                    await _connection.ExecuteAsync(updateQuery, updateParameters);
                }
            }

            // Fetch all columns that are already active in the database, joining with category table to get TableName
            var activeColumnDetailsFromDB = await _connection.QueryAsync<ColumnDetails>(@"
            SELECT ecm.ECMId, ecm.CategoryId, ecm.ColumnDatabaseName, cat.CategoryName AS TableName
            FROM tbl_EmployeeColumnMaster ecm
            JOIN tbl_EmployeeColumnCategoryMaster cat ON ecm.CategoryId = cat.CategoryId
            WHERE ecm.Status = 1"); // Assuming 1 means active

            return activeColumnDetailsFromDB.ToList();
        }
        public async Task<ServiceResponse<byte[]>> DownloadSheetImport(int InstituteId)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    // Fetch data from the database
                    var employeeData = await GetEmployeeDataAsync(InstituteId);
                    var genderData = await GetMasterDataAsync("tbl_Gender");
                    var departmentData = await GetMasterDataAsync("tbl_Department");
                    var designationData = await GetMasterDataAsync("tbl_Designation");
                    var nationalityData = await GetMasterDataAsync("tbl_Nationality");
                    var religionData = await GetMasterDataAsync("tbl_Religion");
                    var maritalStatusData = await GetMasterDataAsync("tbl_MaritalStatus");
                    var bloodGroupData = await GetMasterDataAsync("tbl_BloodGroup");

                    // Create lookup dictionaries for IDs to names
                    var genderLookup = genderData.ToDictionary(g => (int)g.Gender_id, g => g.Gender_Type.ToString());
                    var departmentLookup = departmentData.ToDictionary(d => (int)d.Department_id, d => d.DepartmentName.ToString());
                    var designationLookup = designationData.ToDictionary(d => (int)d.Designation_id, d => d.DesignationName.ToString());
                    var nationalityLookup = nationalityData.ToDictionary(n => (int)n.Nationality_id, n => n.Nationality_Type.ToString());
                    var religionLookup = religionData.ToDictionary(r => (int)r.Religion_id, r => r.Religion_Type.ToString());
                    var maritalStatusLookup = maritalStatusData.ToDictionary(ms => (int)ms.statusId, ms => ms.StatusName.ToString());
                    var bloodGroupLookup = bloodGroupData.ToDictionary(bg => (int)bg.Blood_Group_id, bg => bg.Blood_Group_Type.ToString());

                    // Add sheets to the Excel package
                    var mainSheet = package.Workbook.Worksheets.Add("Employee Data");

                    // Specify the required column order
                    var columnOrder = new[]
                    {
               "Employee_id", "First_Name", "Middle_Name", "Last_Name", "Gender_id", "Department_id", "Designation_id",
                "mobile_number", "Date_of_Birth", "Date_of_Joining", "Religion_id", "Nationality_id",
                "Employee_code_id", "Blood_Group_id", "aadhar_no", "pan_no", "EPF_no", "ESIC_no", "uan_no",
                "Address", "CountryName", "StateName", "CityName", "DistrictName", "Pin_code",
                "Father_Name", "Fathers_Occupation", "Mother_Name", "Mothers_Occupation",
                "Spouse_Name", "Spouses_Occupation", "Guardian_Name", "Guardians_Occupation",
                "Primary_Emergency_Contact_no", "Secondary_Emergency_Contact_no",
                "bank_name", "account_name", "account_number", "IFSC_code", "Bank_address"
            };

                    // Populate the main sheet with dynamic data
                    PopulateSheetWithDynamicData(mainSheet, employeeData, columnOrder,
                        genderLookup, departmentLookup, designationLookup,
                        nationalityLookup, religionLookup, maritalStatusLookup,
                        bloodGroupLookup);
                    // Add master sheets
                    var genderSheet = package.Workbook.Worksheets.Add("Gender");
                    PopulateSheetWithDynamicData(genderSheet, genderData, new string[] { "Gender_id", "Gender_Type" });

                    var departmentSheet = package.Workbook.Worksheets.Add("Department");
                    PopulateSheetWithDynamicData(departmentSheet, departmentData, new string[] { "Department_id", "DepartmentName" });

                    var designationSheet = package.Workbook.Worksheets.Add("Designation");
                    PopulateSheetWithDynamicData(designationSheet, designationData, new string[] { "Designation_id", "DesignationName" });

                    var nationalitySheet = package.Workbook.Worksheets.Add("Nationality");
                    PopulateSheetWithDynamicData(nationalitySheet, nationalityData, new string[] { "Nationality_id", "Nationality_Type" });

                    var religionSheet = package.Workbook.Worksheets.Add("Religion");
                    PopulateSheetWithDynamicData(religionSheet, religionData, new string[] { "Religion_id", "Religion_Type" });

                    var maritalStatusSheet = package.Workbook.Worksheets.Add("Marital Status");
                    PopulateSheetWithDynamicData(maritalStatusSheet, maritalStatusData, new string[] { "statusId", "StatusName" });

                    var bloodGroupSheet = package.Workbook.Worksheets.Add("Blood Group");
                    PopulateSheetWithDynamicData(bloodGroupSheet, bloodGroupData, new string[] { "Blood_Group_id", "Blood_Group_Type" });


                    // Add a static instructions sheet
                    var instructionsSheet = package.Workbook.Worksheets.Add("Instructions");
                    AddStaticInstructions(instructionsSheet);

                    // Generate the Excel file as a byte array
                    var fileData = package.GetAsByteArray();

                    // Set response data
                    return new ServiceResponse<byte[]>(true, "Sheet generated successfully!", fileData, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, null, 500);
            }
        }
        private void PopulateSheetWithDynamicData(
            ExcelWorksheet sheet,
            IEnumerable<dynamic> data,
            string[] columnOrder = null,
            Dictionary<int, dynamic> genderLookup = null,
            Dictionary<int, dynamic> departmentLookup = null,
            Dictionary<int, dynamic> designationLookup = null,
            Dictionary<int, dynamic> nationalityLookup = null,
            Dictionary<int, dynamic> religionLookup = null,
            Dictionary<int, dynamic> maritalStatusLookup = null,
            Dictionary<int, dynamic> bloodGroupLookup = null)
        {
            // Check for null arguments
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Convert data to a list for easier access
            var dataList = data.ToList();
            if (!dataList.Any())
                return; // No data to process

            // Initialize columnOrder if it is null
            if (columnOrder == null || columnOrder.Length == 0)
            {
                columnOrder = dataList.First().Keys.ToArray(); // Define headers from the first item
            }

            // Add column headers
            for (int i = 0; i < columnOrder.Length; i++)
            {
                var columnName = columnOrder[i];
                sheet.Cells[1, i + 1].Value = columnName;

                // Set the background color for mandatory columns
                if (new[] {"Employee_id", "First_Name", "Last_Name", "Gender_id", "Department_id", "Designation_id",
             "mobile_number", "Date_of_Birth", "Date_of_Joining", "Religion_id",
             "Nationality_id", "Employee_code_id", "aadhar_no" }.Contains(columnName))
                {
                    sheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    sheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                }
            }

            // Add data rows
            int rowNumber = 2;
            foreach (var row in dataList)
            {
                if (row == null)
                {
                    Console.WriteLine("Row is null, skipping...");
                    continue; // Skip null rows
                }

                var rowValues = row as IDictionary<string, object>;
                if (rowValues == null)
                {
                    Console.WriteLine("Row values are null, skipping...");
                    continue; // Skip rows that cannot be cast
                }

                for (int col = 0; col < columnOrder.Length; col++)
                {
                    var columnName = columnOrder[col];
                    if (rowValues.ContainsKey(columnName))
                    {
                        var cellValue = rowValues[columnName];

                        // Replace IDs with corresponding names using lookup dictionaries
                        switch (columnName)
                        {
                            case "Gender_id":
                                cellValue = genderLookup?.ContainsKey((int)cellValue) == true ? genderLookup[(int)cellValue] : cellValue;
                                break;
                            case "Department_id":
                                cellValue = departmentLookup?.ContainsKey((int)cellValue) == true ? departmentLookup[(int)cellValue] : cellValue;
                                break;
                            case "Designation_id":
                                cellValue = designationLookup?.ContainsKey((int)cellValue) == true ? designationLookup[(int)cellValue] : cellValue;
                                break;
                            case "Nationality_id":
                                cellValue = nationalityLookup?.ContainsKey((int)cellValue) == true ? nationalityLookup[(int)cellValue] : cellValue;
                                break;
                            case "Religion_id":
                                cellValue = religionLookup?.ContainsKey((int)cellValue) == true ? religionLookup[(int)cellValue] : cellValue;
                                break;
                            case "marrital_status_id":
                                cellValue = maritalStatusLookup?.ContainsKey((int)cellValue) == true ? maritalStatusLookup[(int)cellValue] : cellValue;
                                break;
                            case "Blood_Group_id":
                                cellValue = bloodGroupLookup?.ContainsKey((int)cellValue) == true ? bloodGroupLookup[(int)cellValue] : cellValue;
                                break;
                        }

                        sheet.Cells[rowNumber, col + 1].Value = cellValue ?? DBNull.Value;
                    }
                }
                rowNumber++;
            }

            // Apply some basic formatting
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
        }
        private async Task<IEnumerable<dynamic>> GetEmployeeDataAsync(int instituteId)
        {
            string query = @"
WITH BankDetails AS (
    SELECT 
        employee_id,
        bank_name,
        account_name,
        account_number,
        IFSC_code,
        Bank_address,
        ROW_NUMBER() OVER (PARTITION BY employee_id ORDER BY bank_id) AS rn
    FROM 
        tbl_BankDetailsmaster
),
FamilyDetails AS (
    SELECT 
        Employee_id,
        Father_Name,
        Fathers_Occupation,
        Mother_Name,
        Mothers_Occupation,
        Spouse_Name,
        Spouses_Occupation,
        Guardian_Name,
        Guardians_Occupation,
        Primary_Emergency_Contact_no,
        Secondary_Emergency_Contact_no,
        ROW_NUMBER() OVER (PARTITION BY Employee_id ORDER BY Employee_family_id) AS rn
    FROM 
        tbl_EmployeeFamilyMaster
),
AddressDetails AS (
    SELECT 
        Employee_id,
        Address,
        CountryName,
        StateName,
        CityName,
        DistrictName,
        Pin_code,
        ROW_NUMBER() OVER (PARTITION BY Employee_id ORDER BY Employee_Present_Address_id) AS rn
    FROM 
        tbl_EmployeePresentAddress
)

SELECT 
    e.Employee_id, 
    e.First_Name, 
    e.Middle_Name, 
    e.Last_Name, 
    e.Gender_id, 
    e.Department_id, 
    e.Designation_id, 
    e.mobile_number, 
    e.Date_of_Joining, 
    e.Nationality_id, 
    e.Religion_id, 
    e.Date_of_Birth, 
    e.EmailID, 
    e.Employee_code_id, 
    e.marrital_status_id, 
    e.Blood_Group_id, 
    e.aadhar_no, 
    e.pan_no, 
    e.EPF_no, 
    e.ESIC_no, 
    e.Status, 
    e.uan_no,
    -- Bank Details
    b.bank_name, 
    b.account_name, 
    b.account_number, 
    b.IFSC_code, 
    b.Bank_address,
    -- Family Details
    f.Father_Name, 
    f.Fathers_Occupation, 
    f.Mother_Name, 
    f.Mothers_Occupation, 
    f.Spouse_Name, 
    f.Spouses_Occupation, 
    f.Guardian_Name, 
    f.Guardians_Occupation, 
    f.Primary_Emergency_Contact_no, 
    f.Secondary_Emergency_Contact_no,
    -- Address Details
    a.Address, 
    a.CountryName, 
    a.StateName, 
    a.CityName, 
    a.DistrictName, 
    a.Pin_code
FROM 
    tbl_EmployeeProfileMaster e
LEFT JOIN 
    BankDetails b ON e.Employee_id = b.employee_id AND b.rn = 1
LEFT JOIN 
    FamilyDetails f ON e.Employee_id = f.Employee_id AND f.rn = 1
LEFT JOIN 
    AddressDetails a ON e.Employee_id = a.Employee_id AND a.rn = 1
WHERE 
    e.Institute_id = @InstituteId;
";




            // Execute the query using your database handling method (Dapper/ADO.NET/EF)
            var data = await _connection.QueryAsync<dynamic>(query, new { InstituteId = instituteId });
            return data.ToList();
        }
        private async Task<IEnumerable<dynamic>> GetMasterDataAsync(string tableName)
        {
            string query = $"SELECT * FROM {tableName}";

            // Execute the query using your database handling method (Dapper/ADO.NET/EF)
            var data = await _connection.QueryAsync(query);
            return data.ToList();
        }
        private void AddStaticInstructions(ExcelWorksheet sheet)
        {
            sheet.Cells[1, 1].Value = "Instructions for filling the Employee Data sheet:";
            sheet.Cells[2, 1].Value = "1. All fields marked in red are mandatory.";
            sheet.Cells[3, 1].Value = "2. Please ensure that all mandatory fields are filled before uploading the sheet.";
            sheet.Cells[4, 1].Value = "3. Use the appropriate codes from the master sheets for fields like Gender, Department, etc.";

            // Additional static instructions can be added here
        }
        public async Task<ServiceResponse<int>> UploadEmployeedata(IFormFile file, int instituteId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new ServiceResponse<int>(false, "File is empty or null", 0, 400);
                }

                // Load master data mappings
                var genderMappings = await GetGenderMappings();
                var departmentMappings = await GetDepartmentMappings();
                var designationMappings = await GetDesignationMappings();
                var religionMappings = await GetReligionMappings();
                var nationalityMappings = await GetNationalityMappings();
                var bloodGroupMappings = await GetBloodGroupMappings();

                bool success = true;

                // Read the Excel file
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        // Read data starting from the second row (to skip headers)
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            DateTime? dateOfBirth = ParseDate(worksheet.Cells[row, 9].Value?.ToString());
                            DateTime? dateOfJoining = ParseDate(worksheet.Cells[row, 10].Value?.ToString());
                            var employeeProfile = new EmployeeProfile
                            {
                                Employee_id = Convert.ToInt32(worksheet.Cells[row, 1].Value),
                                First_Name = worksheet.Cells[row, 2].Value?.ToString(),
                                Middle_Name = worksheet.Cells[row, 3].Value?.ToString(),
                                Last_Name = worksheet.Cells[row, 4].Value?.ToString(),

                                // Fetch the ID by looking up the name in the dictionary
                                Gender_id = genderMappings.TryGetValue(worksheet.Cells[row, 5].Value?.ToString(), out var genderId) ? genderId : 0,
                                Department_id = departmentMappings.TryGetValue(worksheet.Cells[row, 6].Value?.ToString(), out var deptId) ? deptId : 0,
                                Designation_id = designationMappings.TryGetValue(worksheet.Cells[row, 7].Value?.ToString(), out var desigId) ? desigId : 0,

                                mobile_number = worksheet.Cells[row, 8].Value?.ToString(),
                                Date_of_Birth = dateOfBirth,
                                Date_of_Joining = dateOfJoining,

                                Religion_id = religionMappings.TryGetValue(worksheet.Cells[row, 11].Value?.ToString(), out var religionId) ? religionId : 0,
                                Nationality_id = nationalityMappings.TryGetValue(worksheet.Cells[row, 12].Value?.ToString(), out var nationalityId) ? nationalityId : 0,

                                Employee_code_id = worksheet.Cells[row, 13].Value?.ToString(),
                                Blood_Group_id = bloodGroupMappings.TryGetValue(worksheet.Cells[row, 14].Value?.ToString(), out var bloodGroupId) ? bloodGroupId : 0,

                                aadhar_no = worksheet.Cells[row, 15].Value?.ToString(),
                                pan_no = worksheet.Cells[row, 16].Value?.ToString(),
                                EPF_no = worksheet.Cells[row, 17].Value?.ToString(),
                                ESIC_no = worksheet.Cells[row, 18].Value?.ToString(),
                                uan_no = worksheet.Cells[row, 19].Value?.ToString(),

                                EmployeeAddressDetails = new List<EmployeeAddressDetails>
                        {
                            new EmployeeAddressDetails
                            {
                                Address = worksheet.Cells[row, 20].Value?.ToString(),
                                CountryName = worksheet.Cells[row, 21].Value?.ToString(),
                                StateName = worksheet.Cells[row, 22].Value?.ToString(),
                                CityName = worksheet.Cells[row, 23].Value?.ToString(),
                                DistrictName = worksheet.Cells[row, 24].Value?.ToString(),
                                Pin_code = worksheet.Cells[row, 25].Value?.ToString(),
                            }
                        },

                                Family = new EmployeeFamily
                                {
                                    Father_Name = worksheet.Cells[row, 26].Value?.ToString(),
                                    Fathers_Occupation = worksheet.Cells[row, 27].Value?.ToString(),
                                    Mother_Name = worksheet.Cells[row, 28].Value?.ToString(),
                                    Mothers_Occupation = worksheet.Cells[row, 29].Value?.ToString(),
                                    Spouse_Name = worksheet.Cells[row, 30].Value?.ToString(),
                                    Spouses_Occupation = worksheet.Cells[row, 31].Value?.ToString(),
                                    Guardian_Name = worksheet.Cells[row, 32].Value?.ToString(),
                                    Guardians_Occupation = worksheet.Cells[row, 33].Value?.ToString(),
                                    Primary_Emergency_Contact_no = worksheet.Cells[row, 34].Value?.ToString(),
                                    Secondary_Emergency_Contact_no = worksheet.Cells[row, 35].Value?.ToString(),
                                },

                                EmployeeBankDetails = new List<EmployeeBankDetails>
                        {
                            new EmployeeBankDetails
                            {
                                bank_name = worksheet.Cells[row, 36].Value?.ToString(),
                                account_name = worksheet.Cells[row, 37].Value?.ToString(),
                                account_number = worksheet.Cells[row, 38].Value?.ToString(),
                                IFSC_code = worksheet.Cells[row, 39].Value?.ToString(),
                                Bank_address = worksheet.Cells[row, 40].Value?.ToString(),
                            }
                        },

                                // Set Institute_id to the profile
                                Institute_id = instituteId
                            };

                            // Call the AddUpdateEmployeeProfile method for each employee
                            var response = await AddUpdateEmployeeProfile(employeeProfile);
                            if (response.Success)
                            {
                                success = true;
                            }
                            else
                            {
                                success = false;
                                break;  // Exit loop on failure
                            }
                        }
                    }
                }

                if (success)
                {
                    return new ServiceResponse<int>(true, "Operation successful", 0, 200);
                }
                else
                {
                    return new ServiceResponse<int>(false, "Operation failed", 0, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        // Methods to fetch master data mappings
        private DateTime? ParseDate(string dateStr)
        {
            if (DateTime.TryParseExact(dateStr, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            return null; // Return null if parsing fails
        }
        private async Task<Dictionary<string, int>> GetGenderMappings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var genders = await connection.QueryAsync<Gender>("SELECT Gender_Type, Gender_id FROM tbl_Gender");
                return genders.ToDictionary(g => g.Gender_Type, g => g.Gender_id);
            }
        }

        private async Task<Dictionary<string, int>> GetDepartmentMappings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var departments = await connection.QueryAsync<Department>("SELECT DepartmentName, Department_id FROM tbl_Department");
                return departments.ToDictionary(d => d.DepartmentName, d => d.Department_id);
            }
        }

        private async Task<Dictionary<string, int>> GetDesignationMappings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var designations = await connection.QueryAsync<Designation>("SELECT DesignationName, Designation_id FROM tbl_Designation");
                return designations.ToDictionary(d => d.DesignationName, d => d.Designation_id);
            }
        }

        private async Task<Dictionary<string, int>> GetReligionMappings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var religions = await connection.QueryAsync<Religion>("SELECT Religion_Type, Religion_id FROM tbl_Religion");
                return religions.ToDictionary(r => r.Religion_Type, r => r.Religion_id);
            }
        }

        private async Task<Dictionary<string, int>> GetNationalityMappings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var nationalities = await connection.QueryAsync<Nationality>("SELECT Nationality_Type, Nationality_id FROM tbl_Nationality");
                return nationalities.ToDictionary(n => n.Nationality_Type, n => n.Nationality_id);
            }
        }

        private async Task<Dictionary<string, int>> GetBloodGroupMappings()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var bloodGroups = await connection.QueryAsync<BloodGroup>("SELECT Blood_Group_Type, Blood_Group_id FROM tbl_BloodGroup");
                return bloodGroups.ToDictionary(b => b.Blood_Group_Type, b => b.Blood_Group_id);
            }
        }
    }
    public class Gender
    {
        public int Gender_id {  get; set; }
        public string Gender_Type {  get; set; }
    }
    public class Religion
    {
        public int Religion_id {  get; set; }
        public string Religion_Type {  get; set; }
    }
    public class Nationality
    {
        public int Nationality_id {  get; set; }
        public string Nationality_Type {  get; set; }
    }
    public class ColumnDetails
    {
        public int ECMId { get; set; }
        public int CategoryId { get; set; }
        public string ColumnDatabaseName { get; set; } // Actual column name in the database
        public string TableName { get; set; }          // The table the column belongs to
    }

}