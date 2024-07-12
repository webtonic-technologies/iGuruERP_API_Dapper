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
                        en_date = request.en_date
                    };
                    string query = @"INSERT INTO [tbl_InstituteDetails] (Institute_name, Institute_Alias, en_date)
                             VALUES (@Institute_name, @Institute_Alias, @en_date)
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
                        int logosInserted = await InsertInstituteLogos(request.InstituteLogos ??= ([]), institutionId);
                        int stampsInserted = await InsertInstituteDigitalStamps(request.InstituteDigitalStamps ??= ([]), institutionId);
                        int signsInserted = await InsertInstituteDigitalSigns(request.InstituteDigitalSigns ??= ([]), institutionId);
                        int prinSignsInserted = await InsertInstitutePrinSigns(request.InstitutePrinSigns ??= ([]), institutionId);
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
                    en_date = @en_date
                WHERE Institute_id = @Institute_id";
                    var newInstitution = new InstituteDetails
                    {
                        Institute_id = request.Institute_id,
                        Institute_name = request.Institute_name,
                        Institute_Alias = request.Institute_Alias,
                        en_date = request.en_date
                    };
                    int rowsAffected = await _connection.ExecuteAsync(updateQuery, newInstitution);
                    if (rowsAffected > 0)
                    {
                        int logosInserted = await InsertInstituteLogos(request.InstituteLogos ??= ([]), request.Institute_id);
                        int stampsInserted = await InsertInstituteDigitalStamps(request.InstituteDigitalStamps ??= ([]), request.Institute_id);
                        int signsInserted = await InsertInstituteDigitalSigns(request.InstituteDigitalSigns ??= ([]), request.Institute_id);
                        int prinSignsInserted = await InsertInstitutePrinSigns(request.InstitutePrinSigns ??= ([]), request.Institute_id);
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

                    var logos = await _connection.QueryAsync<InstituteLogos>(
                        "SELECT * FROM tbl_InstituteLogo WHERE InstituteId = @InstituteId",
                        new { InstituteId = Id });
                    response.InstituteLogos = logos != null ? logos.Select(l => new InstituteLogos
                    {
                        InstituteLogoId = l.InstituteLogoId,
                        InstituteId = l.InstituteId,
                        InstituteLogo = GetImage(l.InstituteLogo)
                    }).ToList() : new List<InstituteLogos>();

                    var stamps = await _connection.QueryAsync<InstituteDigitalStamps>(
                        "SELECT * FROM tbl_InstituteDigitalStamp WHERE InstituteId = @InstituteId",
                        new { InstituteId = Id });
                    response.InstituteDigitalStamps = stamps != null ? stamps.Select(s => new InstituteDigitalStamps
                    {
                        InstituteDigitalStampId = s.InstituteDigitalStampId,
                        InstituteId = s.InstituteId,
                        DigitalStamp = GetImage(s.DigitalStamp)
                    }).ToList() : new List<InstituteDigitalStamps>();

                    var signs = await _connection.QueryAsync<InstituteDigitalSigns>(
                        "SELECT * FROM tbl_InstituteDigitalSign WHERE InstituteId = @InstituteId",
                        new { InstituteId = Id });
                    response.InstituteDigitalSigns = signs != null ? signs.Select(s => new InstituteDigitalSigns
                    {
                        InstituteDigitalSignId = s.InstituteDigitalSignId,
                        InstituteId = s.InstituteId,
                        DigitalSign = GetImage(s.DigitalSign)
                    }).ToList() : new List<InstituteDigitalSigns>();

                    var prinSigns = await _connection.QueryAsync<InstitutePrinSigns>(
                        "SELECT * FROM tbl_InstitutePrinSign WHERE InstituteId = @InstituteId",
                        new { InstituteId = Id });
                    response.InstitutePrinSigns = prinSigns != null ? prinSigns.Select(p => new InstitutePrinSigns
                    {
                        InstitutePrinSignId = p.InstitutePrinSignId,
                        InstituteId = p.InstituteId,
                        InstitutePrinSign = GetImage(p.InstitutePrinSign)
                    }).ToList() : new List<InstitutePrinSigns>();

                    var des = await _connection.QueryFirstOrDefaultAsync<InstituteDescription>(
                        "SELECT * FROM tbl_InstitueDescription WHERE Institute_id = @Institute_id",
                        new { Institute_id = Id });
                    response.InstituteDescription = des ?? new InstituteDescription();

                    var address = await _connection.QueryAsync<InstituteAddress>(
                        "SELECT * FROM tbl_InstituteAddress WHERE Institute_id = @Institute_id",
                        new { Institute_id = Id });
                    response.InstituteAddresses = address != null ? address.AsList() : new List<InstituteAddress>();

                    var smMapping = await _connection.QueryAsync<InstituteSMMapping>(
                        "SELECT * FROM tbl_InstitueSMMapping WHERE Institute_id = @Institute_id",
                        new { Institute_id = Id });
                    response.InstituteSMMappings = smMapping != null ? smMapping.AsList() : new List<InstituteSMMapping>();

                    var schCont = await _connection.QueryAsync<SchoolContact>(
                        "SELECT * FROM tbl_SchoolContact WHERE Institute_id = @Institute_id",
                        new { Institute_id = Id });
                    response.SchoolContacts = schCont != null ? schCont.AsList() : new List<SchoolContact>();

                    var academic = await _connection.QueryAsync<AcademicInfo>(
                        "SELECT * FROM tbl_AcademicInfo WHERE Institute_id = @Institute_id",
                        new { Institute_id = Id });
                    response.AcademicInfos = academic != null ? academic.AsList() : new List<AcademicInfo>();

                    var sem = await _connection.QueryFirstOrDefaultAsync<SemesterInfo>(
                        "SELECT * FROM tbl_SemesterInfo WHERE Institute_id = @Institute_id",
                        new { Institute_id = Id });
                    response.SemesterInfo = sem ?? new SemesterInfo();

                    return new ServiceResponse<InstituteDetailsDTO>(true, "Records found", response, 200);
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
        public async Task<ServiceResponse<bool>> DeleteImage(DeleteImageRequest request)
        {
            try
            {
                string deleteQuery = string.Empty;
                string imagePath = string.Empty;

                // Determine the query and file path based on the ImageName
                switch (request.ImageName)
                {
                    case "Logo":
                        deleteQuery = "DELETE FROM tbl_InstituteLogo WHERE InstituteLogoId = @ImageId AND InstituteId = @InstituteId";
                        imagePath = await _connection.QuerySingleOrDefaultAsync<string>(
                            "SELECT InstituteLogo FROM tbl_InstituteLogo WHERE InstituteLogoId = @ImageId AND InstituteId = @InstituteId",
                            new { request.ImageId, request.InstituteId });
                        break;
                    case "DigStamp":
                        deleteQuery = "DELETE FROM tbl_InstituteDigitalStamp WHERE InstituteDigitalStampId = @ImageId AND InstituteId = @InstituteId";
                        imagePath = await _connection.QuerySingleOrDefaultAsync<string>(
                            "SELECT DigitalStamp FROM tbl_InstituteDigitalStamp WHERE InstituteDigitalStampId = @ImageId AND InstituteId = @InstituteId",
                            new { request.ImageId, request.InstituteId });
                        break;
                    case "DigSign":
                        deleteQuery = "DELETE FROM tbl_InstituteDigitalSign WHERE InstituteDigitalSignId = @ImageId AND InstituteId = @InstituteId";
                        imagePath = await _connection.QuerySingleOrDefaultAsync<string>(
                            "SELECT DigitalSign FROM tbl_InstituteDigitalSign WHERE InstituteDigitalSignId = @ImageId AND InstituteId = @InstituteId",
                            new { request.ImageId, request.InstituteId });
                        break;
                    case "PrincSign":
                        deleteQuery = "DELETE FROM tbl_InstitutePrinSign WHERE InstitutePrinSignId = @ImageId AND InstituteId = @InstituteId";
                        imagePath = await _connection.QuerySingleOrDefaultAsync<string>(
                            "SELECT InstitutePrinSign FROM tbl_InstitutePrinSign WHERE InstitutePrinSignId = @ImageId AND InstituteId = @InstituteId",
                            new { request.ImageId, request.InstituteId });
                        break;
                    default:
                        return new ServiceResponse<bool>(false, "Invalid ImageName", false, 400);
                }

                // Delete the record from the database
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { request.ImageId, request.InstituteId });
                if (rowsAffected > 0)
                {
                    // Delete the file from the folder
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                    return new ServiceResponse<bool>(true, "Image deleted successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Image not found or delete failed", false, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
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
        private async Task<int> InsertInstituteLogos(List<InstituteLogos> logos, int instituteId)
        {
            if (logos == null || instituteId <= 0)
                return 0;

            string deleteQuery = "DELETE FROM tbl_InstituteLogo WHERE InstituteId = @InstituteId";
            await _connection.ExecuteAsync(deleteQuery, new { InstituteId = instituteId });

            foreach (var logo in logos)
            {
                var newLogo = new InstituteLogos
                {
                    InstituteId = instituteId,
                    InstituteLogo = ImageUpload(logo.InstituteLogo)
                };

                string logoQuery = @"
            INSERT INTO tbl_InstituteLogo (InstituteId, InstituteLogo)
            VALUES (@InstituteId, @InstituteLogo);
        ";

                int logoId = await _connection.ExecuteAsync(logoQuery, new { InstituteId = newLogo.InstituteId, InstituteLogo = newLogo.InstituteLogo });
                if (logoId <= 0)
                {
                    return 0;
                }
            }
            return 1;
        }
        private async Task<int> InsertInstituteDigitalStamps(List<InstituteDigitalStamps> stamps, int instituteId)
        {
            if (stamps == null || instituteId <= 0)
                return 0;

            string deleteQuery = "DELETE FROM tbl_InstituteDigitalStamp WHERE InstituteId = @InstituteId";
            await _connection.ExecuteAsync(deleteQuery, new { InstituteId = instituteId });

            foreach (var stamp in stamps)
            {
                var newStamp = new InstituteDigitalStamps
                {
                    InstituteId = instituteId,
                    DigitalStamp = ImageUpload(stamp.DigitalStamp)
                };

                string stampQuery = @"
            INSERT INTO tbl_InstituteDigitalStamp (InstituteId, DigitalStamp)
            VALUES (@InstituteId, @DigitalStamp);
        ";

                int stampId = await _connection.ExecuteAsync(stampQuery, new { InstituteId = newStamp.InstituteId, DigitalStamp = newStamp.DigitalStamp });
                if (stampId <= 0)
                {
                    return 0;
                }
            }
            return 1;
        }
        private async Task<int> InsertInstituteDigitalSigns(List<InstituteDigitalSigns> signs, int instituteId)
        {
            if (signs == null || instituteId <= 0)
                return 0;

            string deleteQuery = "DELETE FROM tbl_InstituteDigitalSign WHERE InstituteId = @InstituteId";
            await _connection.ExecuteAsync(deleteQuery, new { InstituteId = instituteId });

            foreach (var sign in signs)
            {
                var newSign = new InstituteDigitalSigns
                {
                    InstituteId = instituteId,
                    DigitalSign = ImageUpload(sign.DigitalSign)
                };

                string signQuery = @"
            INSERT INTO tbl_InstituteDigitalSign (InstituteId, DigitalSign)
            VALUES (@InstituteId, @DigitalSign);
        ";

                int signId = await _connection.ExecuteAsync(signQuery, new { InstituteId = newSign.InstituteId, DigitalSign = newSign.DigitalSign });
                if (signId <= 0)
                {
                    return 0;
                }
            }
            return 1;
        }
        private async Task<int> InsertInstitutePrinSigns(List<InstitutePrinSigns> prinSigns, int instituteId)
        {
            if (prinSigns == null || instituteId <= 0)
                return 0;

            string deleteQuery = "DELETE FROM tbl_InstitutePrinSign WHERE InstituteId = @InstituteId";
            await _connection.ExecuteAsync(deleteQuery, new { InstituteId = instituteId });

            foreach (var prinSign in prinSigns)
            {
                var newPrinSign = new InstitutePrinSigns
                {
                    InstituteId = instituteId,
                    InstitutePrinSign = ImageUpload(prinSign.InstitutePrinSign)
                };

                string prinSignQuery = @"
            INSERT INTO tbl_InstitutePrinSign (InstituteId, InstitutePrinSign)
            VALUES (@InstituteId, @InstitutePrinSign);
        ";

                int prinSignId = await _connection.ExecuteAsync(prinSignQuery, new { InstituteId = newPrinSign.InstituteId, InstitutePrinSign = newPrinSign.InstitutePrinSign });
                if (prinSignId <= 0)
                {
                    return 0;
                }
            }
            return 1;
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