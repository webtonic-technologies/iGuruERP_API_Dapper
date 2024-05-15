using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;

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
                        Institute_Logo = ImageUpload(request.Institute_Logo),
                        Institute_DigitalSignatory = ImageUpload(request.Institute_DigitalSignatory),
                        Institute_PrincipalSignatory = ImageUpload(request.Institute_PrincipalSignatory),
                        Institute_DigitalStamp = ImageUpload(request.Institute_DigitalStamp),
                        en_date = request.en_date
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
                        int academic = request.AcademicInfos != null ? await AddUpdateAcademicInfo(request.AcademicInfos, institutionId) : 0;
                        int semester = request.SemesterInfo != null ? await AddUpdateSemesterInfo(request.SemesterInfo, institutionId) : 0;
                        if (address > 0 && schContact > 0 && smMapping > 0 && desc > 0 && academic > 0 && semester > 0)
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
                        Institute_DigitalSignatory = ImageUpload(request.Institute_DigitalSignatory),
                        Institute_DigitalStamp = ImageUpload(request.Institute_DigitalStamp),
                        Institute_Logo = ImageUpload(request.Institute_Logo),
                        Institute_PrincipalSignatory = ImageUpload(request.Institute_PrincipalSignatory),
                        en_date = request.en_date
                    };
                    int rowsAffected = await _connection.ExecuteAsync(updateQuery, newInstitution);
                    if (rowsAffected > 0)
                    {
                        int address = request.InstituteAddresses != null ? await AddUpdateInstituteAddress(request.InstituteAddresses, request.Institute_id) : 0;
                        int schContact = request.SchoolContacts != null ? await AddUpdateSchoolContact(request.SchoolContacts, request.Institute_id) : 0;
                        int smMapping = request.InstituteSMMappings != null ? await AddUpdateSMMapping(request.InstituteSMMappings, request.Institute_id) : 0;
                        int desc = request.InstituteDescription != null ? await AddUpdateInstituteDescription(request.InstituteDescription, request.Institute_id) : 0;
                        int academic = request.AcademicInfos != null ? await AddUpdateAcademicInfo(request.AcademicInfos, request.Institute_id) : 0;
                        int semester = request.SemesterInfo != null ? await AddUpdateSemesterInfo(request.SemesterInfo, request.Institute_id) : 0;
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
                SELECT *
                FROM tbl_InstituteDetails
                WHERE Institute_id = @Institute_id";
                var institute = await _connection.QueryFirstOrDefaultAsync<InstituteDetails>(query, new { Institute_id = Id });

                if (institute != null)
                {
                    response.Institute_id = institute.Institute_id;
                    response.Institute_name = institute.Institute_name;
                    response.Institute_Alias = institute.Institute_Alias;
                    response.en_date = institute.en_date;
                    response.Institute_Logo = GetImage(institute.Institute_Logo);
                    response.Institute_DigitalStamp = GetImage(institute.Institute_DigitalStamp);
                    response.Institute_DigitalSignatory = GetImage(institute.Institute_DigitalSignatory);
                    response.Institute_PrincipalSignatory = GetImage(institute.Institute_PrincipalSignatory);

                    var des = await _connection.QueryFirstOrDefaultAsync<InstituteDescription>(
                        "SELECT * FROM tbl_InstitueDescription WHERE Institute_id = @Institute_id"
                        , new { Institute_id = Id });
                    response.InstituteDescription = des ?? new InstituteDescription();

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

                    var academic = await _connection.QueryAsync<AcademicInfo>(
                   "SELECT * FROM tbl_AcademicInfo WHERE Institute_id = @Institute_id"
                  , new { Institute_id = Id });
                    response.AcademicInfos = academic != null ? academic.AsList() : [];

                    var sem = await _connection.QueryFirstOrDefaultAsync<SemesterInfo>(
                 "SELECT * FROM tbl_SemesterInfo WHERE Institute_id = @Institute_id"
                , new { Institute_id = Id });
                    response.SemesterInfo = sem ?? new SemesterInfo();

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
        private async Task<int> AddUpdateAcademicInfo(List<AcademicInfo> request, int InstitutionId)
        {
            int addedRecords = 0;
            if (request != null)
            {
                foreach (var data in request)
                {
                    data.Institute_id = InstitutionId;
                }
            }
            string query = "SELECT COUNT(*) FROM [tbl_AcademicInfo] WHERE Institute_id = @InstituteId";
            int count = await _connection.ExecuteScalarAsync<int>(query, new { InstituteId = InstitutionId });
            if (count > 0)
            {
                string deleteQuery = "DELETE FROM [tbl_AcademicInfo] WHERE Institute_id = @InstituteId";
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { InstituteId = InstitutionId });
                if (rowsAffected > 0)
                {
                    string insertQuery = @"
                INSERT INTO [tbl_AcademicInfo] (Institute_id, [AcademicYearStartMonth], [AcademicYearEndMonth])
                VALUES (@Institute_id, @AcademicYearStartMonth, @AcademicYearEndMonth)";
                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"
                INSERT INTO [tbl_AcademicInfo] (Institute_id, [AcademicYearStartMonth], [AcademicYearEndMonth])
                VALUES (@Institute_id, @AcademicYearStartMonth, @AcademicYearEndMonth)";
                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }
        private async Task<int> AddUpdateSemesterInfo(SemesterInfo request, int InstitutionId)
        {
            int rowsAffected = 0;
            if (request.SemesterInfoId == 0)
            {
                request.Institute_id = InstitutionId;

                var insertQuery = @"
                INSERT INTO tbl_SemesterInfo (Institute_id, IsSemester, SemesterStartDate, SemesterEndDate)
                VALUES (@Institute_id, @IsSemester, @SemesterStartDate, @SemesterEndDate);";
                // Execute the query with parameterized values
                rowsAffected = await _connection.ExecuteAsync(insertQuery, request);
            }
            else
            {
                var updateQuery = @"
    UPDATE tbl_SemesterInfo
    SET Institute_id = @Institute_id,
        IsSemester = @IsSemester,
        SemesterStartDate = @SemesterStartDate,
        SemesterEndDate = @SemesterEndDate
    WHERE SemesterInfoId = @SemesterInfoId;";

                rowsAffected = await _connection.ExecuteAsync(updateQuery, request);
            }
            return rowsAffected;
        }
        private string ImageUpload(string image)
        {
            if (string.IsNullOrEmpty(image) || image == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "InstituteDetails");

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
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "InstituteDetails", Filename);

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
