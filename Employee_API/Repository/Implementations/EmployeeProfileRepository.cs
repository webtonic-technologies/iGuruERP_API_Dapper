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
                        request.Status,
                        EmpPhoto = ImageUpload(request.EmpPhoto)
                    });
                    if (employeeId > 0)
                    {
                        request.Family.Employee_id = employeeId;
                        var empfam = await AddUpdateEmployeeFamily(request.Family ??= new EmployeeFamily());
                        var empdoc = await AddUpdateEmployeeDocuments(request.EmployeeDocuments ??= [], employeeId);
                        var empQua = await AddUpdateEmployeeQualification(request.EmployeeQualifications ??= [], employeeId);
                        var empwork = await AddUpdateEmployeeWorkExp(request.EmployeeWorkExperiences ??= [], employeeId);
                        var empbank = await AddUpdateEmployeeBankDetails(request.EmployeeBankDetails ??= [], employeeId);
                        var empadd = await AddUpdateEmployeeAddressDetails(request.EmployeeAddressDetails, employeeId);
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
                        request.Status,
                        EmpPhoto = ImageUpload(request.EmpPhoto)
                    });
                    if (rowsAffected > 0)
                    {
                        request.Family.Employee_id = request.Employee_id;
                        var empfam = await AddUpdateEmployeeFamily(request.Family ??= new EmployeeFamily());
                        var empdoc = await AddUpdateEmployeeDocuments(request.EmployeeDocuments ??= [], request.Employee_id);
                        var empQua = await AddUpdateEmployeeQualification(request.EmployeeQualifications ??= [], request.Employee_id);
                        var empwork = await AddUpdateEmployeeWorkExp(request.EmployeeWorkExperiences ??= [], request.Employee_id);
                        var empbank = await AddUpdateEmployeeBankDetails(request.EmployeeBankDetails ??= [], request.Employee_id);
                        var empadd = await AddUpdateEmployeeAddressDetails(request.EmployeeAddressDetails, request.Employee_id);
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
        public async Task<ServiceResponse<int>> AddUpdateEmployeeDocuments(List<EmployeeDocument> request, int employee_id)
        {
            try
            {
                foreach(var data in request)
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
                INSERT INTO tbl_EmployeePresentAddress (Address, Country_id, State_id, City_id, District_id, Pin_code, AddressTypeId, Employee_id)
                VALUES (@Address, @Country_id, @State_id, @City_id, @District_id, @Pin_code, @AddressTypeId, @Employee_id);";

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
                // Base SQL query with joins to fetch names including nationality and religion
                string sql = @"
            SELECT ep.Employee_id,
                   ep.First_Name,
                   ep.Middle_Name,
                   ep.Last_Name,
                   ep.Gender_id,
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
            WHERE ep.Institute_id = @InstituteId";

                // Initialize parameters
                var parameters = new DynamicParameters();
                parameters.Add("InstituteId", request.InstituteId);

                // Conditionally apply filters
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

                // Implement pagination
                sql += " ORDER BY ep.Employee_id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
                parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
                parameters.Add("PageSize", request.PageSize);

                // Execute the query and retrieve the list of employee profiles
                var employees = await _connection.QueryAsync<EmployeeProfileResponseDTO>(sql, parameters);

                // Check if any records were found
                if (employees != null && employees.Any())
                {
                    return new ServiceResponse<List<EmployeeProfileResponseDTO>>(true, "Records found", employees.ToList(), 200);
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
            e.Country_id,
            c.country_name as CountryName,
            e.State_id,
            s.state_name as StateName,
            e.City_id,
            ci.city_name as CityName,
            e.District_id,
            d.district_name as DistrictName,
            e.Pin_code,
            e.AddressTypeId,
            e.Employee_id
        FROM tbl_EmployeePresentAddress e
        LEFT JOIN tbl_Country c ON e.Country_id = c.Country_id
        LEFT JOIN tbl_State s ON e.State_id = s.State_id
        LEFT JOIN tbl_City ci ON e.City_id = ci.City_id
        LEFT JOIN tbl_District d ON e.District_id = d.District_id
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
        public async Task<ServiceResponse<bool>> StatusActiveInactive(int employeeId)
        {
            try
            {
                var data = await GetEmployeeProfileById(employeeId);

                if (data.Data != null)
                {
                    bool Status = !data.Data.Status;

                    string sql = "UPDATE tbl_EmployeeProfileMaster SET Status = @Status WHERE Employee_id = @Employee_id";

                    int rowsAffected = await _connection.ExecuteAsync(sql, new { Status , Employee_id = employeeId });
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