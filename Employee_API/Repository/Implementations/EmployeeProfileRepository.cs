using Dapper;
using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Models;
using Employee_API.Repository.Interfaces;
using System.Data;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Employee_API.Repository.Implementations
{
    public class EmployeeProfileRepository : IEmployeeProfileRepository
    {
        private readonly IDbConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EmployeeProfileRepository(IDbConnection connection, IWebHostEnvironment hostingEnvironment)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<ServiceResponse<string>> AddUpdateEmployeeProfile(EmployeeProfileDTO request)
        {
            try
            {
                if (request.Employee_id == 0)
                {
                    string sql = @"INSERT INTO [dbo].[tbl_EmployeeProfileMaster] 
                        (First_Name, Middle_Name, Last_Name, Gender_id, Department_id, Designation_id, mobile_number, 
                         Date_of_Joining, Nationality_id, Religion_id, Date_of_Birth, EmailID, Employee_code_id, marrital_status_id, 
                         Blood_Group_id, aadhar_no, pan_no, EPF_no, ESIC_no, Institute_id) 
                       VALUES 
                        (@FirstName, @MiddleName, @LastName, @GenderId, @DepartmentId, @DesignationId, @MobileNumber, 
                         @DateOfJoining, @NationalityId, @ReligionId, @DateOfBirth, @EmailID, @EmployeeCodeId, @MarritalStatusId, 
                         @BloodGroupId, @AadharNo, @PanNo, @EPFNo, @ESICNo, @InstituteId);
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
                        request.Date_of_Joining,
                        request.Nationality_id,
                        request.Religion_id,
                        request.Date_of_Birth,
                        request.EmailID,
                        request.Employee_code_id,
                        request.marrital_status_id,
                        request.Blood_Group_id,
                        request.aadhar_no,
                        request.pan_no,
                        request.EPF_no,
                        request.ESIC_no,
                        request.Institute_id
                    });
                    if (employeeId > 0)
                    {
                        int bank = await AddUpdateEmployeeBankDetails(request.EmployeeBankDetails, employeeId);
                        int doc = await AddUpdateEmployeeDecuments(request.EmployeeDocuments, employeeId);
                        int fam = await AddUpdateEmployeeFamily(request.Family, employeeId);
                        int qua = await AddUpdateEmployeeQualification(request.EmployeeQualifications, employeeId);
                        int exp = await AddUpdateEmployeeWorkExp(request.EmployeeWorkExperiences, employeeId);
                        if (bank > 0 || doc > 0 || fam > 0 || qua > 0 || exp > 0)
                        {
                            return new ServiceResponse<string>(true, "operation successful", "Record added successfully", 200);
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
                else
                {
                    string sql = @"UPDATE [dbo].[tbl_EmployeeProfileMaster] SET 
                        First_Name = @FirstName, 
                        Middle_Name = @MiddleName, 
                        Last_Name = @LastName, 
                        Gender_id = @GenderId, 
                        Department_id = @DepartmentId, 
                        Designation_id = @DesignationId, 
                        mobile_number = @MobileNumber, 
                        Date_of_Joining = @DateOfJoining, 
                        Nationality_id = @NationalityId, 
                        Religion_id = @ReligionId, 
                        Date_of_Birth = @DateOfBirth, 
                        EmailID = @EmailID, 
                        Employee_code_id = @EmployeeCodeId, 
                        marrital_status_id = @MarritalStatusId, 
                        Blood_Group_id = @BloodGroupId, 
                        aadhar_no = @AadharNo, 
                        pan_no = @PanNo, 
                        EPF_no = @EPFNo, 
                        ESIC_no = @ESICNo, 
                        Institute_id = @InstituteId
                      WHERE Employee_id = @EmployeeId";

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
                        request.Date_of_Joining,
                        request.Nationality_id,
                        request.Religion_id,
                        request.Date_of_Birth,
                        request.EmailID,
                        request.Employee_code_id,
                        request.marrital_status_id,
                        request.Blood_Group_id,
                        request.aadhar_no,
                        request.pan_no,
                        request.EPF_no,
                        request.ESIC_no,
                        request.Institute_id
                    });
                    if (rowsAffected > 0)
                    {
                        int bank = await AddUpdateEmployeeBankDetails(request.EmployeeBankDetails, request.Employee_id);
                        int doc = await AddUpdateEmployeeDecuments(request.EmployeeDocuments, request.Employee_id);
                        int fam = await AddUpdateEmployeeFamily(request.Family, request.Employee_id);
                        int qua = await AddUpdateEmployeeQualification(request.EmployeeQualifications, request.Employee_id);
                        int exp = await AddUpdateEmployeeWorkExp(request.EmployeeWorkExperiences, request.Employee_id);
                        if (bank > 0 || doc > 0 || fam > 0 || qua > 0 || exp > 0)
                        {
                            return new ServiceResponse<string>(true, "operation successful", "Record updated successfully", 200);
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

        public async Task<ServiceResponse<EmployeeProfileResponseDTO>> GetEmployeeProfileById(int employeeId)
        {
            try
            {
                var response = new EmployeeProfileResponseDTO();
                string sql = @"SELECT Employee_id, First_Name, Middle_Name, Last_Name, Gender_id, Department_id, 
                            Designation_id, mobile_number, Date_of_Joining, Nationality_id, Religion_id, 
                            Date_of_Birth, EmailID, Employee_code_id, marrital_status_id, Blood_Group_id, 
                            aadhar_no, pan_no, EPF_no, ESIC_no, Institute_id
                       FROM [dbo].[tbl_EmployeeProfileMaster]
                       WHERE Employee_id = @EmployeeId";

                // Execute the query and retrieve the employee profile
                var employee = await _connection.QueryFirstOrDefaultAsync<EmployeeProfile>(sql, new { EmployeeId = employeeId });
                if(employee != null)
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

                    string famsql = @"SELECT Employee_family_id, Employee_id, Father_Name, Fathers_Occupation,
                            Mother_Name, Mothers_Occupation, Spouse_Name, Spouses_Occupation,
                            Guardian_Name, Guardians_Occupation, Primary_Emergency_Contact_no,
                            Secondary_Emergency_Contact_no
                       FROM [dbo].[tbl_EmployeeFamilyMaster]
                       WHERE Employee_id = @EmployeeId";

                    // Execute the query and retrieve the employee family details
                    var employeeFamily = await _connection.QueryFirstOrDefaultAsync<EmployeeFamily>(famsql, new { EmployeeId = employeeId });
                    if(employeeFamily != null)
                    {
                        response.Family = employeeFamily;
                    }
                    string banksql = @"SELECT bank_id, employee_id, bank_name, account_name, account_number, IFSC_code, Bank_address
                       FROM [dbo].[tbl_BankDetailsmaster]
                       WHERE employee_id = @EmployeeId";

                    // Execute the query and retrieve the list of bank details
                    var bankDetails = await _connection.QueryAsync<EmployeeBankDetails>(banksql, new { EmployeeId = employeeId });
                    if(bankDetails != null)
                    {
                        response.EmployeeBankDetails = bankDetails.AsList();
                    }
                    string docsql = @"SELECT Document_id, employee_id, Document_Name, file_name, file_path
                       FROM [dbo].[tbl_DocumentsMaster]
                       WHERE employee_id = @EmployeeId";

                    // Execute the query and retrieve the list of documents
                    var documents = await _connection.QueryAsync<EmployeeDocument>(docsql, new { EmployeeId = employeeId });
                    if (documents != null)
                    {
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
                    return new ServiceResponse<EmployeeProfileResponseDTO>(false, "Records found", response , 200);
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
        public async Task<ServiceResponse<List<EmployeeProfile>>> GetEmployeeProfileList(int InstituteId)
        {
            try
            {
                string sql = @"SELECT Employee_id, First_Name, Middle_Name, Last_Name, Gender_id, Department_id, 
                            Designation_id, mobile_number, Date_of_Joining, Nationality_id, Religion_id, 
                            Date_of_Birth, EmailID, Employee_code_id, marrital_status_id, Blood_Group_id, 
                            aadhar_no, pan_no, EPF_no, ESIC_no, Institute_id
                       FROM [dbo].[tbl_EmployeeProfileMaster]
                       WHERE Institute_id = @InstituteId";

                // Execute the query and retrieve the list of employees
                var employees = await _connection.QueryAsync<EmployeeProfile>(sql, new { InstituteId });
                if (employees != null)
                {
                    return new ServiceResponse<List<EmployeeProfile>>(true, "operation successful", employees.AsList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<EmployeeProfile>>(false, "Some error occured", [], 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EmployeeProfile>>(false, ex.Message, [], 500);
            }
        }
        private async Task<int> AddUpdateEmployeeBankDetails(List<EmployeeBankDetails>? request, int employeeId)
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
                        (@EmployeeId, @BankName, @AccountName, @AccountNumber, @IFSCCode, @BankAddress);"; // Retrieve the inserted bank_id

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"INSERT INTO [dbo].[tbl_BankDetailsmaster] 
                        (employee_id, bank_name, account_name, account_number, IFSC_code, Bank_address) 
                       VALUES 
                        (@EmployeeId, @BankName, @AccountName, @AccountNumber, @IFSCCode, @BankAddress);"; // Retrieve the inserted bank_id

                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }
        private async Task<int> AddUpdateEmployeeDecuments(List<EmployeeDocumentDTO>? request, int employeeId)
        {
            int addedRecords = 0;
            if (request != null)
            {
                foreach (var data in request)
                {
                    data.employee_id = employeeId;
                }
            }
            string query = "SELECT COUNT(*) FROM tbl_DocumentsMaster WHERE employee_id = @employee_id";
            int count = await _connection.ExecuteScalarAsync<int>(query, new { employee_id = employeeId });
            if (count > 0)
            {
                string deleteQuery = "DELETE FROM tbl_DocumentsMaster WHERE employee_id = @employee_id";
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { employee_id = employeeId });
                if (rowsAffected > 0)
                {
                    string insertQuery = @"INSERT INTO [dbo].[tbl_DocumentsMaster] 
                        (employee_id, Document_Name, file_name, file_path) 
                       VALUES 
                        (@EmployeeId, @DocumentName, @FileName, @FilePath);";
                    var records = new List<EmployeeDocument>();
                    foreach (var item in request)
                    {
                        var data = new EmployeeDocument
                        {
                            Document_id = item.Document_id,
                            Document_Name = item.Document_Name,
                            employee_id = employeeId,
                            file_name = item.file_name,
                            file_path = await HandleImageUpload(item.file_path)
                        };
                        records.Add(data);
                    }

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, records);
                }
            }
            else
            {
                string insertQuery = @"INSERT INTO [dbo].[tbl_DocumentsMaster] 
                        (employee_id, Document_Name, file_name, file_path) 
                       VALUES 
                        (@EmployeeId, @DocumentName, @FileName, @FilePath);";
                var records = new List<EmployeeDocument>();
                foreach (var item in request)
                {
                    var data = new EmployeeDocument
                    {
                        Document_id = item.Document_id,
                        Document_Name = item.Document_Name,
                        employee_id = employeeId,
                        file_name = item.file_name,
                        file_path = await HandleImageUpload(item.file_path)
                    };
                    records.Add(data);
                }

                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, records);
            }
            return addedRecords;
        }
        private async Task<int> AddUpdateEmployeeFamily(EmployeeFamily? request, int employeeId)
        {
            int rowsAffected;
            if (request.Employee_family_id == 0)
            {
                string sql = @"INSERT INTO [dbo].[tbl_EmployeeFamilyMaster] 
                        (Employee_id, Father_Name, Fathers_Occupation, Mother_Name, Mothers_Occupation, 
                         Spouse_Name, Spouses_Occupation, Guardian_Name, Guardians_Occupation, 
                         Primary_Emergency_Contact_no, Secondary_Emergency_Contact_no) 
                       VALUES 
                        (@EmployeeId, @FatherName, @FathersOccupation, @MotherName, @MothersOccupation, 
                         @SpouseName, @SpousesOccupation, @GuardianName, @GuardiansOccupation, 
                         @PrimaryEmergencyContactNo, @SecondaryEmergencyContactNo);";

                // Execute the query and retrieve the inserted Employee_family_id
                rowsAffected = await _connection.ExecuteAsync(sql, new
                {
                    employeeId,
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
                        Father_Name = @FatherName, Fathers_Occupation = @FathersOccupation, 
                        Mother_Name = @MotherName, Mothers_Occupation = @MothersOccupation, 
                        Spouse_Name = @SpouseName, Spouses_Occupation = @SpousesOccupation, 
                        Guardian_Name = @GuardianName, Guardians_Occupation = @GuardiansOccupation, 
                        Primary_Emergency_Contact_no = @PrimaryEmergencyContactNo, 
                        Secondary_Emergency_Contact_no = @SecondaryEmergencyContactNo 
                      WHERE Employee_family_id = @EmployeeFamilyId";

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
            return rowsAffected;
        }
        private async Task<int> AddUpdateEmployeeQualification(List<EmployeeQualification>? request, int employeeId)
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
                        (@EmployeeId, @EducationalQualification, @YearOfCompletion);";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"INSERT INTO [dbo].[tbl_QualificationInfoMaster] 
                        (employee_id, Educational_Qualification, Year_of_Completion) 
                       VALUES 
                        (@EmployeeId, @EducationalQualification, @YearOfCompletion);";

                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }
        private async Task<int> AddUpdateEmployeeWorkExp(List<EmployeeWorkExperience>? request, int employeeId)
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
                        (@Year, @Month, @PreviousOrganisation, @PreviousDesignation, @EmployeeId);";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"INSERT INTO [dbo].[tbl_WorkExperienceMaster] 
                        (Year, Month, Previous_Organisation, Previous_Designation, Employee_id) 
                       VALUES 
                        (@Year, @Month, @PreviousOrganisation, @PreviousDesignation, @EmployeeId);";

                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }
        private async Task<string> HandleImageUpload(IFormFile request)
        {
            if (request != null)
            {
                var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Employee");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var fileName = Path.GetFileNameWithoutExtension(request.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(request.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await request.CopyToAsync(fileStream);
                }
                return fileName;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

