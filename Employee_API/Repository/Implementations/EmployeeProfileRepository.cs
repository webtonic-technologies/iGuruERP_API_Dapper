using Dapper;
using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Models;
using Employee_API.Repository.Interfaces;
using System.Data;

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
        public async Task<ServiceResponse<int>> AddUpdateEmployeeProfile(EmployeeProfile request)
        {
            try
            {
                if (request.Employee_id == 0)
                {
                    string sql = @"INSERT INTO [dbo].[tbl_EmployeeProfileMaster] 
                        (First_Name, Middle_Name, Last_Name, Gender_id, Department_id, Designation_id, mobile_number, 
                         Date_of_Joining, Nationality_id, Religion_id, Date_of_Birth, EmailID, Employee_code_id, marrital_status_id, 
                         Blood_Group_id, aadhar_no, pan_no, EPF_no, ESIC_no, Institute_id, EmpPhoto, uan_no) 
                       VALUES 
                        (@First_Name, @Middle_Name, @Last_Name, @Gender_id, @Department_id, @Designation_id, @mobile_number, 
                         @Date_of_Joining, @Nationality_id, @Religion_id, @Date_of_Birth, @EmailID, @Employee_code_id, @marrital_status_id, 
                         @Blood_Group_id, @aadhar_no, @pan_no, @EPF_no, @ESIC_no, @Institute_id, @EmpPhoto, @uan_no);
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
                        request.Institute_id,
                        request.uan_no,
                        EmpPhoto = ImageUpload(request.EmpPhoto)
                    });
                    if (employeeId > 0)
                    {
                        return new ServiceResponse<int>(true, "operation successful", employeeId, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
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
                        uan_no = @uan_no
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
                        request.Institute_id,
                        request.uan_no,
                        EmpPhoto = ImageUpload(request.EmpPhoto)
                    });
                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<int>(true, "operation successful", request.Employee_id, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
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
        public async Task<ServiceResponse<string>> AddUpdateEmployeeDocuments(List<EmployeeDocument> request, int employee_id)
        {
            try
            {
                int addedRecords = 0;
                string query = "SELECT COUNT(*) FROM tbl_DocumentsMaster WHERE employee_id = @employee_id";
                int count = await _connection.ExecuteScalarAsync<int>(query, new { employee_id });
                if (count > 0)
                {
                    string deleteQuery = "DELETE FROM tbl_DocumentsMaster WHERE employee_id = @employee_id";
                    int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { employee_id });
                    if (rowsAffected > 0)
                    {
                        string insertQuery = @"INSERT INTO [dbo].[tbl_DocumentsMaster] 
                        (employee_id, Document_Name, file_name, file_path) 
                       VALUES 
                        (@employee_id, @Document_Name, @file_name, @file_path);";
                        foreach (EmployeeDocument document in request)
                        {
                            document.file_path = ImageUpload(document.file_path);
                        }
                        addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                        if (addedRecords > 0)
                        {
                            return new ServiceResponse<string>(true, "operation successful", "Documents added successfully", 200);
                        }
                        else
                        {
                            return new ServiceResponse<string>(false, "operation successful", string.Empty, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                    }
                }
                else
                {
                    string insertQuery = @"INSERT INTO [dbo].[tbl_DocumentsMaster] 
                        (employee_id, Document_Name, file_name, file_path) 
                       VALUES 
                        (@employee_id, @Document_Name, @file_name, @file_path);";
                    foreach (EmployeeDocument document in request)
                    {
                        document.file_path = ImageUpload(document.file_path);
                    }
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                    if (addedRecords > 0)
                    {
                        return new ServiceResponse<string>(true, "operation successful", "Documents added successfully", 200);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "operation successful", string.Empty, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
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
        public async Task<ServiceResponse<EmployeeProfileResponseDTO>> GetEmployeeProfileById(int employeeId)
        {
            try
            {
                var response = new EmployeeProfileResponseDTO();
                string sql = @"SELECT Employee_id, First_Name, Middle_Name, Last_Name, Gender_id, Department_id, 
                            Designation_id, mobile_number, Date_of_Joining, Nationality_id, Religion_id, 
                            Date_of_Birth, EmailID, Employee_code_id, marrital_status_id, Blood_Group_id, 
                            aadhar_no, pan_no, EPF_no, ESIC_no, Institute_id, EmpPhoto, uan_no
                       FROM [dbo].[tbl_EmployeeProfileMaster]
                       WHERE Employee_id = @EmployeeId";

                // Execute the query and retrieve the employee profile
                var employee = await _connection.QueryFirstOrDefaultAsync<EmployeeProfile>(sql, new { EmployeeId = employeeId });
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
                    [iGuruERP].[dbo].[tbl_EmployeeFamilyMaster]
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
        public async Task<ServiceResponse<List<EmployeeProfile>>> GetEmployeeProfileList(GetAllEmployeeListRequest request)
        {
            try
            {
                string sql = @"SELECT Employee_id, First_Name, Middle_Name, Last_Name, Gender_id, Department_id, 
                            Designation_id, mobile_number, Date_of_Joining, Nationality_id, Religion_id, 
                            Date_of_Birth, EmailID, Employee_code_id, marrital_status_id, Blood_Group_id, 
                            aadhar_no, pan_no, EPF_no, ESIC_no, Institute_id
                       FROM [dbo].[tbl_EmployeeProfileMaster]
                       WHERE Institute_id = @InstituteId
                       AND (@DepartmentId IS 0 OR [Department_id] = @DepartmentId)
                       AND (@DesignationId IS 0 OR [Designation_id] = @DesignationId)";

                // Execute the query and retrieve the list of employees
                var employees = await _connection.QueryAsync<EmployeeProfile>(sql, new { request });
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
        public async Task<ServiceResponse<List<EmployeeDocument>>> GetEmployeeDocuments(int employee_id)
        {
            try
            {
                var response = new List<EmployeeDocument>();
                var data = await _connection.QueryAsync<EmployeeDocument>(
                   "SELECT * FROM tbl_DocumentsMaster WHERE employee_id = @employee_id",
                   new { employee_id }) ?? throw new Exception("Data not found");
                string filePath = string.Empty;

                foreach (var item in data)
                {
                    item.file_path = GetImage(item.file_path);
                }
                return new ServiceResponse<List<EmployeeDocument>>(true, "Record Found", response, 200);
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
                    [iGuruERP].[dbo].[tbl_QualificationInfoMaster]
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
                    [iGuruERP].[dbo].[tbl_WorkExperienceMaster]
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
                    [iGuruERP].[dbo].[tbl_BankDetailsmaster]
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
        private string ImageUpload(string image)
        {
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "EmployeeProfile");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileExtension = IsJpeg(imageData) == true ? ".jpg" : IsPng(imageData) == true ? ".png" : IsGif(imageData) == true ? ".gif" : string.Empty;
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(directoryPath, fileName);

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
                throw new Exception("File not found");
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
    }
}

