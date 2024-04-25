using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;
using System.Net;
using System.Xml.Serialization;

namespace Institute_API.Repository.Implementations
{
    public class InstituteDetailsRepository : IInstituteDetailsRepository
    {
        private readonly IDbConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public InstituteDetailsRepository(IDbConnection connection, IWebHostEnvironment hostingEnvironment)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<ServiceResponse<int>> AddUpdateInstititeDetails(InstituteDetailsDTO request)
        {
            try
            {
                if (request.Institute_id == 0)
                {
                    var newInstitution = new InstituteDetails
                    {
                        Institute_name = request.Institute_name,
                        Institute_Alias = request.Institute_Alias,
                        Institute_Logo = string.Empty,
                        Institute_DigitalSignatory = string.Empty,
                        Institute_PrincipalSignatory = string.Empty,
                        Institute_DigitalStamp = string.Empty,
                    };
                    string query = @"INSERT INTO [tbl_InstituteDetails] (Institute_name, Institute_Alias, Institute_Logo, Institute_DigitalStamp, Institute_DigitalSignatory, Institute_PrincipalSignatory, en_date)
                             VALUES (@Institute_name, @Institute_Alias, @Institute_Logo, @Institute_DigitalStamp, @Institute_DigitalSignatory, @Institute_PrincipalSignatory, @en_date)
                             SELECT SCOPE_IDENTITY()";

                    int institutionId = await _connection.QuerySingleOrDefaultAsync<int>(query, newInstitution);
                    if (institutionId > 0)
                    {
                        int address = request.InstituteAddresses != null ? await AddUpdateInstituteAddress(request.InstituteAddresses, institutionId) : 0;
                        int schContact = request.SchoolContacts != null ? await AddUpdateSchoolContact(request.SchoolContacts, institutionId) : 0;
                        int smMapping = request.InstituteSMMappings != null ? await AddUpdateSMMapping(request.InstituteSMMappings, institutionId) : 0;
                        int desc = request.InstituteDescription != null ? await AddUpdateInstituteDescription(request.InstituteDescription, institutionId) : 0;
                        if (address > 0 && schContact > 0 && smMapping > 0 && desc > 0)
                        {
                            return new ServiceResponse<int>(true, "Operation successsful", institutionId, 200);
                        }
                        else
                        {
                            return new ServiceResponse<int>(false, "Operation failed", 0, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Operation failed", 0, 500);
                    }
                }
                else
                {
                    string updateQuery = @"
                UPDATE tbl_InstituteDetails
                SET 
                    Institute_name = @Institute_name,
                    Institute_Alias = @Institute_Alias,
                    Institute_Logo = @Institute_Logo,
                    Institute_DigitalStamp = @Institute_DigitalStamp,
                    Institute_DigitalSignatory = @Institute_DigitalSignatory,
                    Institute_PrincipalSignatory = @Institute_PrincipalSignatory,
                    en_date = @en_date
                WHERE Institute_id = @Institute_id";
                    var newInstitution = new InstituteDetails
                    {
                        Institute_id = request.Institute_id,
                        Institute_name = request.Institute_name,
                        Institute_Alias = request.Institute_Alias,
                        Institute_DigitalSignatory = string.Empty,
                        Institute_DigitalStamp = string.Empty,
                        Institute_Logo = string.Empty,
                        Institute_PrincipalSignatory = string.Empty
                    };
                    int rowsAffected = await _connection.ExecuteAsync(updateQuery, newInstitution);
                    if (rowsAffected > 0)
                    {
                        int address = request.InstituteAddresses != null ? await AddUpdateInstituteAddress(request.InstituteAddresses, request.Institute_id) : 0;
                        int schContact = request.SchoolContacts != null ? await AddUpdateSchoolContact(request.SchoolContacts, request.Institute_id) : 0;
                        int smMapping = request.InstituteSMMappings != null ? await AddUpdateSMMapping(request.InstituteSMMappings, request.Institute_id) : 0;
                        int desc = request.InstituteDescription != null ? await AddUpdateInstituteDescription(request.InstituteDescription, request.Institute_id) : 0;
                        if (address > 0 && schContact > 0 && smMapping > 0 && desc > 0)
                        {
                            return new ServiceResponse<int>(true, "Operation successsful", request.Institute_id, 200);
                        }
                        else
                        {
                            return new ServiceResponse<int>(false, "Operation failed", 0, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Operation failed", 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<InstituteDetailsDTO>> GetInstituteDetailsById(int Id)
        {
            try
            {
                var response = new InstituteDetailsDTO();
                string query = @"
                SELECT Institute_id, Institute_name, Institute_Alias, en_date
                FROM tbl_InstituteDetails
                WHERE Institute_id = @Institute_id";
                var institute = await _connection.QueryFirstOrDefaultAsync<InstituteDetails>(query, new { Institute_id = Id });

                if (institute != null)
                {
                    response.Institute_id = institute.Institute_id;
                    response.Institute_name = institute.Institute_name;
                    response.Institute_Alias = institute.Institute_Alias;
                    response.en_date = institute.en_date;

                    var des = await _connection.QueryFirstOrDefaultAsync<InstituteDescription>(
                        "SELECT * FROM tbl_InstitueDescription WHERE Institute_id = @Institute_id"
                        , new { Institute_id = Id });
                    response.InstituteDescription = des != null ? des : new InstituteDescription();

                    var address = await _connection.QueryAsync<InstituteAddress>(
                         "SELECT * FROM tbl_InstituteAddress WHERE Institute_id = @Institute_id"
                        , new { Institute_id = Id });
                    response.InstituteAddresses = address != null ? address.AsList() : [];

                    var smMapping = await _connection.QueryAsync<InstituteSMMapping>(
                      "SELECT * FROM tbl_InstitueSMMapping WHERE Institute_id = @Institute_id"
                     , new { Institute_id = Id });
                    response.InstituteSMMappings = smMapping != null ? smMapping.AsList() : [];

                    var schCont = await _connection.QueryAsync<SchoolContact>(
                     "SELECT * FROM tbl_SchoolContact WHERE Institute_id = @Institute_id"
                    , new { Institute_id = Id });
                    response.SchoolContacts = schCont != null ? schCont.AsList() : [];

                    return new ServiceResponse<InstituteDetailsDTO>(true, " records found", response, 200);
                }
                else
                {
                    return new ServiceResponse<InstituteDetailsDTO>(false, "No records found", new InstituteDetailsDTO(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteDetailsDTO>(false, ex.Message, new InstituteDetailsDTO(), 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> GetInstituteLogoById(int Id)
        {
            try
            {
                var data = await _connection.QueryFirstOrDefaultAsync<InstituteDetails>(
                   "SELECT Institute_Logo FROM tbl_InstituteDetails WHERE Institute_id = @Institute_id",
                   new { Institute_id = Id }) ?? throw new Exception("Data not found");
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution", data.Institute_Logo);

                if (!File.Exists(filePath))
                    throw new Exception("File not found");
                var fileBytes = await File.ReadAllBytesAsync(filePath);

                return new ServiceResponse<byte[]>(true, "Record Found", fileBytes, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> GetInstituteDigitalStampById(int Id)
        {
            try
            {
                var data = await _connection.QueryFirstOrDefaultAsync<InstituteDetails>(
                   "SELECT Institute_DigitalStamp FROM tbl_InstituteDetails WHERE Institute_id = @Institute_id",
                   new { Institute_id = Id }) ?? throw new Exception("Data not found");
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution", data.Institute_DigitalStamp);

                if (!File.Exists(filePath))
                    throw new Exception("File not found");
                var fileBytes = await File.ReadAllBytesAsync(filePath);

                return new ServiceResponse<byte[]>(true, "Record Found", fileBytes, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> GetInstituteDigitalSignatoryById(int Id)
        {
            try
            {
                var data = await _connection.QueryFirstOrDefaultAsync<InstituteDetails>(
                   "SELECT Institute_DigitalSignatory FROM tbl_InstituteDetails WHERE Institute_id = @Institute_id",
                   new { Institute_id = Id }) ?? throw new Exception("Data not found");
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution", data.Institute_DigitalSignatory);

                if (!File.Exists(filePath))
                    throw new Exception("File not found");
                var fileBytes = await File.ReadAllBytesAsync(filePath);

                return new ServiceResponse<byte[]>(true, "Record Found", fileBytes, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> GetInstitutePrincipalSignatoryById(int Id)
        {
            try
            {
                var data = await _connection.QueryFirstOrDefaultAsync<InstituteDetails>(
                   "SELECT Institute_PrincipalSignatory FROM tbl_InstituteDetails WHERE Institute_id = @Institute_id",
                   new { Institute_id = Id }) ?? throw new Exception("Data not found");
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution", data.Institute_PrincipalSignatory);

                if (!File.Exists(filePath))
                    throw new Exception("File not found");
                var fileBytes = await File.ReadAllBytesAsync(filePath);

                return new ServiceResponse<byte[]>(true, "Record Found", fileBytes, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<string>> AddUpdateInstituteLogo(InstLogoDTO request)
        {
            try
            {
                string sql = @"UPDATE [dbo].[tbl_InstituteDetails]
                       SET Institute_Logo = @LogoFileName
                       WHERE Institute_id = @InstituteId";
                string logoFileName = request.InstLogo != null ? await HandleImageUpload(request.InstLogo) : string.Empty;
                // Execute the update query

                int rowsAffected = await _connection.ExecuteAsync(sql, new { LogoFileName = logoFileName, InstituteId = request.Institute_id });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successsful", "Logo added successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation failed", string.Empty, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
        public async Task<ServiceResponse<string>> AddUpdatePrincipalSignatory(InstPriSignDTO request)
        {
            try
            {
                string sql = @"UPDATE [dbo].[tbl_InstituteDetails]
                       SET Institute_PrincipalSignatory = @Institute_PrincipalSignatory
                       WHERE Institute_id = @InstituteId";
                string Institute_PrincipalSignatory = request.InstPrinSign != null ? await HandleImageUpload(request.InstPrinSign) : string.Empty;
                int rowsAffected = await _connection.ExecuteAsync(sql, new { Institute_PrincipalSignatory, InstituteId = request.Institute_id });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successsful", "Signatory added successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation failed", string.Empty, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
        public async Task<ServiceResponse<string>> AddUpdateDigitalSignatory(InstDigSignDTO request)
        {
            try
            {
                string sql = @"UPDATE [dbo].[tbl_InstituteDetails]
                       SET Institute_DigitalSignatory = @Institute_DigitalSignatory
                       WHERE Institute_id = @InstituteId";
                string Institute_DigitalSignatory = request.InstDigSign != null ? await HandleImageUpload(request.InstDigSign) : string.Empty;
                int rowsAffected = await _connection.ExecuteAsync(sql, new { Institute_DigitalSignatory, InstituteId = request.Institute_id });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successsful", "Signatory added successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation failed", string.Empty, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
        public async Task<ServiceResponse<string>> AddUpdateDigitalStamp(InstDigiStampDTO request)
        {
            try
            {
                string sql = @"UPDATE [dbo].[tbl_InstituteDetails]
                       SET Institute_DigitalStamp = @Institute_DigitalStamp
                       WHERE Institute_id = @InstituteId";
                string Institute_DigitalStamp = request.InstDigStamp != null ? await HandleImageUpload(request.InstDigStamp) : string.Empty;
                int rowsAffected = await _connection.ExecuteAsync(sql, new { Institute_DigitalStamp, InstituteId = request.Institute_id });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successsful", "Stamp added successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation failed", string.Empty, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
        //public async Task<ServiceResponse<byte[]>> GetInstituteFileById(int Id, string fileType)
        //{
        //    try
        //    {
        //        string columnName = "";
        //        columnName = fileType.ToLower() switch
        //        {
        //            "logo" => "Institute_Logo",
        //            "digitalstamp" => "Institute_DigitalStamp",
        //            "digitalsignatory" => "Institute_DigitalSignatory",
        //            "principalsignatory" => "Institute_PrincipalSignatory",
        //            _ => throw new ArgumentException("Invalid file type specified"),
        //        };
        //        var data = await _connection.QueryFirstOrDefaultAsync<InstituteDetails>(
        //            $"SELECT {columnName} FROM tbl_InstituteDetails WHERE Institute_id = @Institute_id",
        //            new { Institute_id = Id }) ?? throw new Exception("Data not found");

        //        var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution", data.GetType().GetProperty(columnName).GetValue(data).ToString());

        //        if (!File.Exists(filePath))
        //            throw new Exception("File not found");

        //        var fileBytes = await File.ReadAllBytesAsync(filePath);

        //        return new ServiceResponse<byte[]>(true, "Record Found", fileBytes, 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<byte[]>(false, ex.Message, new byte[0], 500);
        //    }
        //}
        private async Task<int> AddUpdateInstituteAddress(List<InstituteAddress> request, int InstitutionId)
        {
            int addedRecords = 0;
            if (request != null)
            {
                foreach (var data in request)
                {
                    data.Institute_id = InstitutionId;
                }
            }
            string query = "SELECT COUNT(*) FROM tbl_InstituteAddress WHERE Institute_id = @InstituteId";
            int count = await _connection.ExecuteScalarAsync<int>(query, new { InstituteId = InstitutionId });
            if (count > 0)
            {
                string deleteQuery = "DELETE FROM tbl_InstituteAddress WHERE Institute_id = @InstituteId";
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { InstituteId = InstitutionId });
                if (rowsAffected > 0)
                {
                    string insertQuery = @"
                INSERT INTO tbl_InstituteAddress (Institute_id, country_id, state_id, city_id, house, pincode, district_id, Locality, Landmark, Mobile_number, Email, en_date, AddressType_id)
                VALUES (@Institute_id, @country_id, @state_id, @city_id, @house, @pincode, @district_id, @Locality, @Landmark, @Mobile_number, @Email, @en_date, @AddressType_id)";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"
                INSERT INTO tbl_InstituteAddress (Institute_id, country_id, state_id, city_id, house, pincode, district_id, Locality, Landmark, Mobile_number, Email, en_date, AddressType_id)
                VALUES (@Institute_id, @country_id, @state_id, @city_id, @house, @pincode, @district_id, @Locality, @Landmark, @Mobile_number, @Email, @en_date, @AddressType_id)";

                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }
        private async Task<int> AddUpdateSchoolContact(List<SchoolContact> request, int InstitutionId)
        {
            int addedRecords = 0;
            if (request != null)
            {
                foreach (var data in request)
                {
                    data.Institute_id = InstitutionId;
                }
            }
            string query = "SELECT COUNT(*) FROM tbl_SchoolContact WHERE Institute_id = @InstituteId";
            int count = await _connection.ExecuteScalarAsync<int>(query, new { InstituteId = InstitutionId });
            if (count > 0)
            {
                string deleteQuery = "DELETE FROM tbl_SchoolContact WHERE Institute_id = @InstituteId";
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { InstituteId = InstitutionId });
                if (rowsAffected > 0)
                {
                    string insertQuery = @"
                INSERT INTO tbl_SchoolContact (ContactType_id, Institute_id, Contact_Person_name, Telephone_number, Email_ID, Mobile_number, isPrimary, en_date)
                VALUES (@ContactType_id, @Institute_id, @Contact_Person_name, @Telephone_number, @Email_ID, @Mobile_number, @isPrimary, @en_date)";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"
                INSERT INTO tbl_SchoolContact (ContactType_id, Institute_id, Contact_Person_name, Telephone_number, Email_ID, Mobile_number, isPrimary, en_date)
                VALUES (@ContactType_id, @Institute_id, @Contact_Person_name, @Telephone_number, @Email_ID, @Mobile_number, @isPrimary, @en_date)";

                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }
        private async Task<int> AddUpdateSMMapping(List<InstituteSMMapping> request, int InstitutionId)
        {
            int addedRecords = 0;
            if (request != null)
            {
                foreach (var data in request)
                {
                    data.Institute_id = InstitutionId;
                }
            }
            string query = "SELECT COUNT(*) FROM tbl_InstitueSMMapping WHERE Institute_id = @InstituteId";
            int count = await _connection.ExecuteScalarAsync<int>(query, new { InstituteId = InstitutionId });
            if (count > 0)
            {
                string deleteQuery = "DELETE FROM tbl_InstitueSMMapping WHERE Institute_id = @InstituteId";
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { InstituteId = InstitutionId });
                if (rowsAffected > 0)
                {
                    string insertQuery = @"
                INSERT INTO tbl_InstitueSMMapping (Institute_id, SM_Id, URL)
                VALUES (@Institute_id, @SM_Id, @URL)";
                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"
                INSERT INTO tbl_InstitueSMMapping (Institute_id, SM_Id, URL)
                VALUES (@Institute_id, @SM_Id, @URL)";
                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }
        private async Task<int> AddUpdateInstituteDescription(InstituteDescription request, int InstitutionId)
        {
            int rowsAffected = 0;
            if (request.Institute_description_id == 0)
            {
                request.Institute_id = InstitutionId;
                string insertQuery = @"
                INSERT INTO tbl_InstitueDescription (Institute_id, Introduction, Mission_Statement, Vision)
                VALUES (@Institute_id, @Introduction, @Mission_Statement, @Vision)";

                // Execute the query with parameterized values
                rowsAffected = await _connection.ExecuteAsync(insertQuery, request);
            }
            else
            {
                string updateQuery = @"
                UPDATE tbl_InstitueDescription 
                SET Introduction = @Introduction, 
                    Mission_Statement = @Mission_Statement, 
                    Vision = @Vision
                WHERE Institute_description_id = @Institute_description_id";

                rowsAffected = await _connection.ExecuteAsync(updateQuery, request);
            }
            return rowsAffected;
        }
        private async Task<string> HandleImageUpload(IFormFile request)
        {
            if (request != null)
            {
                var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution");
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
