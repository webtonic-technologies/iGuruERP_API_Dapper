using Dapper;
using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Models;
using Employee_API.Repository.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Data.SqlClient;

namespace Employee_API.Repository.Implementations
{
    public class EmployeeProfileRepository : IEmployeeProfileRepository
    {
        private readonly IDbConnection _connection;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EmployeeProfileRepository(IDbConnection connection, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
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
                    int rowsAffected = await _connection.ExecuteAsync(sql, new
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
                        var empadd = await AddUpdateEmployeeAddressDetails(request.EmployeeAddressDetails, request.Employee_id);
                        request.EmployeeStaffMappingRequest.EmployeeId = request.Employee_id;
                        var mapp = await AddUpdateEmployeeStaffMapping(request.EmployeeStaffMappingRequest);
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
        //public async Task<ServiceResponse<int>> AddUpdateEmployeeProfile(EmployeeProfile request)
        //{
        //    try
        //    {
        //        if (request.Employee_id == 0)
        //        {
        //            string sql = @"INSERT INTO [dbo].[tbl_EmployeeProfileMaster] 
        //                (First_Name, Middle_Name, Last_Name, Gender_id, Department_id, Designation_id, mobile_number, 
        //                 Date_of_Joining, Nationality_id, Religion_id, Date_of_Birth, EmailID, Employee_code_id, marrital_status_id, 
        //                 Blood_Group_id, aadhar_no, pan_no, EPF_no, ESIC_no, Institute_id, EmpPhoto, uan_no, Status) 
        //               VALUES 
        //                (@First_Name, @Middle_Name, @Last_Name, @Gender_id, @Department_id, @Designation_id, @mobile_number, 
        //                 @Date_of_Joining, @Nationality_id, @Religion_id, @Date_of_Birth, @EmailID, @Employee_code_id, @marrital_status_id, 
        //                 @Blood_Group_id, @aadhar_no, @pan_no, @EPF_no, @ESIC_no, @Institute_id, @EmpPhoto, @uan_no, @Status);
        //               SELECT SCOPE_IDENTITY();"; // Retrieve the inserted Employee_id

        //            // Execute the query and retrieve the inserted Employee_id
        //            int employeeId = await _connection.ExecuteScalarAsync<int>(sql, new
        //            {
        //                request.First_Name,
        //                request.Middle_Name,
        //                request.Last_Name,
        //                request.Gender_id,
        //                request.Department_id,
        //                request.Designation_id,
        //                request.mobile_number,
        //                request.Date_of_Joining,
        //                request.Nationality_id,
        //                request.Religion_id,
        //                request.Date_of_Birth,
        //                request.EmailID,
        //                request.Employee_code_id,
        //                request.marrital_status_id,
        //                request.Blood_Group_id,
        //                request.aadhar_no,
        //                request.pan_no,
        //                request.EPF_no,
        //                request.ESIC_no,
        //                request.Institute_id,
        //                request.uan_no,
        //                request.Status,
        //                EmpPhoto = ImageUpload(request.EmpPhoto)
        //            });
        //            if (employeeId > 0)
        //            {
        //                request.Family.Employee_id = employeeId;
        //                var empfam = await AddUpdateEmployeeFamily(request.Family ??= new EmployeeFamily());
        //                var empdoc = await AddUpdateEmployeeDocuments(request.EmployeeDocuments ??= [], employeeId);
        //                var empQua = await AddUpdateEmployeeQualification(request.EmployeeQualifications ??= [], employeeId);
        //                var empwork = await AddUpdateEmployeeWorkExp(request.EmployeeWorkExperiences ??= [], employeeId);
        //                var empbank = await AddUpdateEmployeeBankDetails(request.EmployeeBankDetails ??= [], employeeId);
        //                var empadd = await AddUpdateEmployeeAddressDetails(request.EmployeeAddressDetails, employeeId);
        //                var userlog = await CreateUserLoginInfo(employeeId, 1, request.Institute_id);
        //                return new ServiceResponse<int>(true, "operation successful", employeeId, 200);
        //            }
        //            else
        //            {
        //                return new ServiceResponse<int>(false, "Some error occured", 0, 500);
        //            }
        //        }
        //        else
        //        {
        //            string sql = @"UPDATE [dbo].[tbl_EmployeeProfileMaster] SET 
        //                First_Name = @First_Name, 
        //                Middle_Name = @Middle_Name, 
        //                Last_Name = @Last_Name, 
        //                Gender_id = @Gender_id, 
        //                Department_id = @Department_id, 
        //                Designation_id = @Designation_id, 
        //                mobile_number = @mobile_number, 
        //                Date_of_Joining = @Date_of_Joining, 
        //                Nationality_id = @Nationality_id, 
        //                Religion_id = @Religion_id, 
        //                Date_of_Birth = @Date_of_Birth, 
        //                EmailID = @EmailID, 
        //                Employee_code_id = @Employee_code_id, 
        //                marrital_status_id = @marrital_status_id, 
        //                Blood_Group_id = @Blood_Group_id, 
        //                aadhar_no = @aadhar_no, 
        //                pan_no = @pan_no, 
        //                EPF_no = @EPF_no, 
        //                ESIC_no = @ESIC_no, 
        //                Institute_id = @Institute_id,
        //                EmpPhoto = @EmpPhoto,
        //                uan_no = @uan_no,
        //                Status = @Status
        //              WHERE Employee_id = @Employee_id";

        //            // Execute the query
        //            int rowsAffected = await _connection.ExecuteAsync(sql, new
        //            {
        //                request.Employee_id,
        //                request.First_Name,
        //                request.Middle_Name,
        //                request.Last_Name,
        //                request.Gender_id,
        //                request.Department_id,
        //                request.Designation_id,
        //                request.mobile_number,
        //                request.Date_of_Joining,
        //                request.Nationality_id,
        //                request.Religion_id,
        //                request.Date_of_Birth,
        //                request.EmailID,
        //                request.Employee_code_id,
        //                request.marrital_status_id,
        //                request.Blood_Group_id,
        //                request.aadhar_no,
        //                request.pan_no,
        //                request.EPF_no,
        //                request.ESIC_no,
        //                request.Institute_id,
        //                request.uan_no,
        //                request.Status,
        //                EmpPhoto = ImageUpload(request.EmpPhoto)
        //            });
        //            if (rowsAffected > 0)
        //            {
        //                request.Family.Employee_id = request.Employee_id;
        //                var empfam = await AddUpdateEmployeeFamily(request.Family ??= new EmployeeFamily());
        //                var empdoc = await AddUpdateEmployeeDocuments(request.EmployeeDocuments ??= [], request.Employee_id);
        //                var empQua = await AddUpdateEmployeeQualification(request.EmployeeQualifications ??= [], request.Employee_id);
        //                var empwork = await AddUpdateEmployeeWorkExp(request.EmployeeWorkExperiences ??= [], request.Employee_id);
        //                var empbank = await AddUpdateEmployeeBankDetails(request.EmployeeBankDetails ??= [], request.Employee_id);
        //                var empadd = await AddUpdateEmployeeAddressDetails(request.EmployeeAddressDetails, request.Employee_id);
        //                return new ServiceResponse<int>(true, "operation successful", request.Employee_id, 200);
        //            }
        //            else
        //            {
        //                return new ServiceResponse<int>(false, "Some error occured", 0, 500);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<int>(false, ex.Message, 0, 500);
        //    }
        //}
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

            using (var connection = _connection)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete existing addresses for the employee
                        string deleteAddressSql = @"
                DELETE FROM tbl_EmployeePresentAddress 
                WHERE Employee_id = @EmployeeId;";

                        await connection.ExecuteAsync(deleteAddressSql, new { EmployeeId = employeeId }, transaction);

                        // Insert new addresses
                        string insertAddressSql = @"
                INSERT INTO tbl_EmployeePresentAddress (Address, CountryName, StateName, CityName, DistrictName, Pin_code, AddressTypeId, Employee_id)
                VALUES (@Address, @CountryName, @StateName, @CityName, @DistrictName, @Pin_code, @AddressTypeId, @Employee_id);";

                        // Execute the insert operation for each address
                        foreach (var address in request)
                        {
                            await connection.ExecuteAsync(insertAddressSql, address, transaction);
                        }

                        // Commit the transaction
                        transaction.Commit();

                        return new ServiceResponse<int>(true, "Address saved successfully.", request.Count, 200);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<int>(false, ex.Message, 0, 500);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
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
        public async Task<ServiceResponse<List<EmployeeProfileResponseDTO>>> GetEmployeeProfileList(GetAllEmployeeListRequest request)
        {
            try
            {
                // Start the base SQL query
                string sql = @"SELECT DISTINCT ep.Employee_id, 
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
        WHERE ep.Institute_id = @InstituteId";

                var parameters = new DynamicParameters();
                parameters.Add("InstituteId", request.InstituteId);

                // Add conditional filters with proper checks for existing conditions
                if (request.DepartmentId > 0)
                {
                    sql += " AND ep.Department_id = @DepartmentId";
                    parameters.Add("DepartmentId", request.DepartmentId);
                }

                if (request.DesignationId > 0)
                {
                    sql += " AND ep.Designation_id = @DesignationId";
                    parameters.Add("DesignationId", request.DesignationId);
                }

                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    sql += " AND (ep.First_Name LIKE @SearchText OR ep.Last_Name LIKE @SearchText OR ep.EmailID LIKE @SearchText)";
                    parameters.Add("SearchText", $"%{request.SearchText}%");
                }

                // Query execution
                var employees = await _connection.QueryAsync<EmployeeProfileResponseDTO>(sql, parameters);

                if (employees != null && employees.Any())
                {
                    // Apply pagination manually using C# logic
                    var paginatedEmployees = employees
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .Distinct() // Ensure distinct records
                        .ToList();
                    foreach (var data in paginatedEmployees)
                    {
                        var doc = await GetEmployeeDocuments(data.Employee_id);
                        var qua = await GetEmployeeQualificationById(data.Employee_id);
                        var work = await GetEmployeeWorkExperienceById(data.Employee_id);
                        var bank = await GetEmployeeBankDetailsById(data.Employee_id);
                        var fam = await GetEmployeeFamilyDetailsById(data.Employee_id);
                        var add = await GetEmployeeAddressDetailsById(data.Employee_id);
                        var mapping = await GetEmployeeMappingById(data.Employee_id);
                        data.EmployeeDocuments = doc.Data;
                        data.EmployeeQualifications = qua.Data;
                        data.EmployeeWorkExperiences = work.Data;
                        data.EmployeeBankDetails = bank.Data;
                        data.Family = fam.Data;
                        data.EmployeeAddressDetails = add.Data;
                        data.EmployeeStaffMappingResponse = mapping.Data;
                    }
                    return new ServiceResponse<List<EmployeeProfileResponseDTO>>(true, "Records found", paginatedEmployees, 200, paginatedEmployees.Count);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeProfileResponseDTO>>(false, "No records found", new List<EmployeeProfileResponseDTO>(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeProfileResponseDTO>>(false, ex.Message, new List<EmployeeProfileResponseDTO>(), 500);
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
        public async Task<ServiceResponse<bool>> StatusActiveInactive(int employeeId)
        {
            try
            {
                var data = await GetEmployeeProfileById(employeeId);

                if (data.Data != null)
                {
                    bool Status = !data.Data.Status;

                    string sql = "UPDATE tbl_EmployeeProfileMaster SET Status = @Status WHERE Employee_id = @Employee_id";

                    int rowsAffected = await _connection.ExecuteAsync(sql, new { Status, Employee_id = employeeId });
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
        public async Task<ServiceResponse<byte[]>> ExcelDownload(ExcelDownloadRequest request)
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

                // If no records found, create Excel file with only headers
                if (!employeeProfiles.Any())
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

                        // Convert to byte array
                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var fileData = stream.ToArray();
                        return new ServiceResponse<byte[]>(true, "No records found, only headers included.", fileData, StatusCodes.Status200OK);
                    }
                }

                // Create Excel file with data
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

                    // Convert to byte array
                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    var fileData = stream.ToArray();
                    return new ServiceResponse<byte[]>(true, "Excel generated successfully.", fileData, StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, null, StatusCodes.Status500InternalServerError);
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
            SELECT TOP (1) [Employee_id], [First_Name], [Last_Name], [mobile_number]
            FROM [tbl_EmployeeProfileMaster]
            WHERE [Employee_id] = @UserId";

                string studentSql = @"
            SELECT TOP (1) [student_id], [First_Name], [Last_Name], [Admission_Number]
            FROM [tbl_StudentMaster]
            WHERE [student_id] = @UserId";

                // Initialize variables
                string username = null;
                dynamic userDetails = null;

                // Fetch user details based on the UserType
                if (userType == 1) // Employee
                {
                    userDetails = await connection.QueryFirstOrDefaultAsync<dynamic>(employeeSql, new { UserId = userId });
                    if (userDetails != null)
                    {
                        // Construct username for employee
                        string firstName = userDetails.First_Name;
                        string lastName = userDetails.Last_Name;
                        string phoneNumber = userDetails.mobile_number;

                        // Ensure the first name and last name have at least 3 characters and the phone number has at least 4 characters
                        if (firstName.Length >= 3 && lastName.Length >= 3 && phoneNumber.Length >= 4)
                        {
                            username = $"{firstName.Substring(0, 3)}{lastName.Substring(0, 3)}{phoneNumber.Substring(phoneNumber.Length - 4)}";
                        }
                        else
                        {
                            throw new Exception("First name, last name, or phone number too short for username creation.");
                        }
                    }
                }
                else if (userType == 2) // Student
                {
                    userDetails = await connection.QueryFirstOrDefaultAsync<dynamic>(studentSql, new { UserId = userId });
                    if (userDetails != null)
                    {
                        // Construct username for student
                        string firstName = userDetails.First_Name;
                        string lastName = userDetails.Last_Name;
                        string admissionNumber = userDetails.Admission_Number;

                        // Ensure the first name and last name have at least 3 characters and the admission number has at least 4 characters
                        if (firstName.Length >= 3 && lastName.Length >= 3 && admissionNumber.Length >= 4)
                        {
                            username = $"{firstName.Substring(0, 3)}{lastName.Substring(0, 3)}{admissionNumber.Substring(admissionNumber.Length - 4)}";
                        }
                        else
                        {
                            throw new Exception("First name, last name, or admission number too short for username creation.");
                        }
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
    }
}