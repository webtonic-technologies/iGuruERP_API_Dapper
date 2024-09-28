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
                        en_date = DateTime.Now
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
                        en_date = DateTime.Now
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
        public async Task<ServiceResponse<InstituteDetailsResponseDTO>> GetInstituteDetailsById(int Id)
        {
            try
            {
                string queryInstituteDetails = @"
            SELECT 
                i.Institute_id, 
                i.Institute_name, 
                i.Institute_Alias, 
                i.en_date
            FROM tbl_InstituteDetails i
            WHERE i.Institute_id = @Institute_id";

                var institute = await _connection.QueryFirstOrDefaultAsync<InstituteDetails>(queryInstituteDetails, new { Institute_id = Id });

                if (institute != null)
                {
                    var response = new InstituteDetailsResponseDTO
                    {
                        Institute_id = institute.Institute_id,
                        Institute_name = institute.Institute_name,
                        Institute_Alias = institute.Institute_Alias,
                        en_date = institute.en_date,
                        InstituteLogos = await GetInstituteLogos(Id),
                        InstituteDigitalStamps = await GetInstituteDigitalStamps(Id),
                        InstituteDigitalSigns = await GetInstituteDigitalSigns(Id),
                        InstitutePrinSigns = await GetInstitutePrinSigns(Id),
                        InstituteDescription = await GetInstituteDescription(Id),
                        AddressResponse = await GetInstituteAddresses(Id),
                        InstituteSMMappings = await GetInstituteSMMappings(Id),
                        SchoolContacts = await GetSchoolContacts(Id),
                        AcademicInfos = await GetAcademicInfos(Id),
                        SemesterInfo = await GetSemesterInfo(Id)
                    };

                    return new ServiceResponse<InstituteDetailsResponseDTO>(true, "Records found", response, 200);
                }
                else
                {
                    return new ServiceResponse<InstituteDetailsResponseDTO>(false, "No records found", new InstituteDetailsResponseDTO(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteDetailsResponseDTO>(false, ex.Message, new InstituteDetailsResponseDTO(), 500);
            }
        }
        public async Task<ServiceResponse<List<InstituteDetailsResponseDTO>>> GetAllInstituteDetailsList(int AcademicYearId)
        {
            try
            {
                string queryInstitutes = @"
            SELECT 
                i.Institute_id, 
                i.Institute_name, 
                i.Institute_Alias, 
                i.en_date
            FROM tbl_InstituteDetails i
            JOIN tbl_AcademicInfo ai ON i.Institute_id = ai.Institute_id
            JOIN tbl_AcademicYear ay ON ai.AcademicYearStartMonth >= ay.StartDate
                AND ai.AcademicYearEndMonth <= ay.EndDate
            WHERE ay.Id = @AcademicYearId";

                // Fetch the institutes that match the selected academic year
                var institutes = await _connection.QueryAsync<InstituteDetails>(queryInstitutes, new { AcademicYearId });

                var responseList = new List<InstituteDetailsResponseDTO>();

                foreach (var institute in institutes)
                {
                    var response = new InstituteDetailsResponseDTO
                    {
                        Institute_id = institute.Institute_id,
                        Institute_name = institute.Institute_name,
                        Institute_Alias = institute.Institute_Alias,
                        en_date = institute.en_date,
                        InstituteLogos = await GetInstituteLogos(institute.Institute_id),
                        InstituteDigitalStamps = await GetInstituteDigitalStamps(institute.Institute_id),
                        InstituteDigitalSigns = await GetInstituteDigitalSigns(institute.Institute_id),
                        InstitutePrinSigns = await GetInstitutePrinSigns(institute.Institute_id),
                        AddressResponse = await GetInstituteAddresses(institute.Institute_id),
                        InstituteDescription = await GetInstituteDescription(institute.Institute_id),
                        InstituteSMMappings = await GetInstituteSMMappings(institute.Institute_id),
                        SchoolContacts = await GetSchoolContacts(institute.Institute_id),
                        AcademicInfos = await GetAcademicInfos(institute.Institute_id),
                        SemesterInfo = await GetSemesterInfo(institute.Institute_id)
                    };

                    responseList.Add(response);
                }

                return new ServiceResponse<List<InstituteDetailsResponseDTO>>(true, "Records found", responseList, 200, responseList.Count);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<InstituteDetailsResponseDTO>>(false, ex.Message, new List<InstituteDetailsResponseDTO>(), 500);
            }
        }
        public async Task<ServiceResponse<List<Country>>> GetCountriesAsync()
        {
            string query = "SELECT * FROM tbl_Country";
            var data = await _connection.QueryAsync<Country>(query);
            return new ServiceResponse<List<Country>>(true, "Records found", data.ToList(), 200);
        }
        public async Task<ServiceResponse<List<State>>> GetStatesByCountryIdAsync(int countryId)
        {
            string query = "SELECT * FROM tbl_State WHERE Country_id = @Country_id";
            var data = await _connection.QueryAsync<State>(query, new { Country_id = countryId });
            return new ServiceResponse<List<State>>(true, "Records found", data.ToList(), 200);
        }
        public async Task<ServiceResponse<List<City>>> GetCitiesByDistrictIdAsync(int districtId)
        {
            string query = "SELECT * FROM tbl_City WHERE district_id = @district_id";
            var data = await _connection.QueryAsync<City>(query, new { district_id = districtId });
            return new ServiceResponse<List<City>>(true, "Records found", data.ToList(), 200);
        }
        public async Task<ServiceResponse<List<District>>> GetDistrictsByStateIdAsync(int stateId)
        {
            string query = "SELECT * FROM tbl_District WHERE state_id = @state_id";
            var data = await _connection.QueryAsync<District>(query, new { state_id = stateId });
            return new ServiceResponse<List<District>>(true, "Records found", data.ToList(), 200);
        }
        private async Task<List<InstituteLogosResponse>> GetInstituteLogos(int instituteId)
        {
            string query = "SELECT InstituteLogoId, InstituteId, InstituteLogo FROM tbl_InstituteLogo WHERE InstituteId = @InstituteId";
            var logos = await _connection.QueryAsync<InstituteLogos>(query, new { InstituteId = instituteId });
            return logos.Select(l => new InstituteLogosResponse
            {
                InstituteLogoId = l.InstituteLogoId,
                InstituteId = l.InstituteId,
                InstituteLogo = GetImage(l.InstituteLogo)
            }).ToList();
        }
        private async Task<List<InstituteDigitalStampsResponse>> GetInstituteDigitalStamps(int instituteId)
        {
            string query = "SELECT InstituteDigitalStampId, InstituteId, DigitalStamp FROM tbl_InstituteDigitalStamp WHERE InstituteId = @InstituteId";
            var stamps = await _connection.QueryAsync<InstituteDigitalStamps>(query, new { InstituteId = instituteId });
            return stamps.Select(s => new InstituteDigitalStampsResponse
            {
                InstituteDigitalStampId = s.InstituteDigitalStampId,
                InstituteId = s.InstituteId,
                DigitalStamp = GetImage(s.DigitalStamp)
            }).ToList();
        }
        private async Task<List<InstituteDigitalSignsResponse>> GetInstituteDigitalSigns(int instituteId)
        {
            string query = "SELECT InstituteDigitalSignId, InstituteId, DigitalSign FROM tbl_InstituteDigitalSign WHERE InstituteId = @InstituteId";
            var signs = await _connection.QueryAsync<InstituteDigitalSigns>(query, new { InstituteId = instituteId });
            return signs.Select(s => new InstituteDigitalSignsResponse
            {
                InstituteDigitalSignId = s.InstituteDigitalSignId,
                InstituteId = s.InstituteId,
                DigitalSign = GetImage(s.DigitalSign)
            }).ToList();
        }
        private async Task<List<InstitutePrinSignsResponse>> GetInstitutePrinSigns(int instituteId)
        {
            string query = "SELECT InstitutePrinSignId, InstituteId, InstitutePrinSign FROM tbl_InstitutePrinSign WHERE InstituteId = @InstituteId";
            var prinSigns = await _connection.QueryAsync<InstitutePrinSigns>(query, new { InstituteId = instituteId });
            return prinSigns.Select(p => new InstitutePrinSignsResponse
            {
                InstitutePrinSignId = p.InstitutePrinSignId,
                InstituteId = p.InstituteId,
                InstitutePrinSign = GetImage(p.InstitutePrinSign)
            }).ToList();
        }
        private async Task<InstituteDescription> GetInstituteDescription(int instituteId)
        {
            string query = "SELECT * FROM tbl_InstitueDescription WHERE Institute_id = @Institute_id";
            var description = await _connection.QueryFirstOrDefaultAsync<InstituteDescription>(query, new { Institute_id = instituteId });
            return description ?? new InstituteDescription();
        }
        private async Task<InstituteAddressResponse> GetInstituteAddresses(int instituteId)
        {
            string query = @"
    SELECT 
        ia.Institute_address_id, ia.country_id, c.country_name AS CountryName,
        ia.state_id, s.state_name AS StateName, ia.city_id, ci.city_name AS CityName,
        ia.house, ia.pincode, ia.district_id, d.district_name AS DistrictName,
        ia.Locality, ia.Landmark, ia.Mobile_number, ia.Email,
        ia.AddressType_id, at.Address_Type AS AddressTypeName, ia.en_date, ia.Institute_id
    FROM tbl_InstituteAddress ia
    LEFT JOIN tbl_Country c ON ia.country_id = c.Country_id
    LEFT JOIN tbl_State s ON ia.state_id = s.State_id
    LEFT JOIN tbl_City ci ON ia.city_id = ci.City_id
    LEFT JOIN tbl_District d ON ia.district_id = d.District_id
    LEFT JOIN tbl_AddressType at ON ia.AddressType_id = at.AddressType_id
    WHERE ia.Institute_id = @Institute_id";

            var addresses = await _connection.QueryAsync(query, new { Institute_id = instituteId });

            // Initialize the response
            var addressResponse = new InstituteAddressResponse
            {
                BillingAddress = new List<AddressResponse>(),
                MailingAddress = new List<AddressResponse>()
            };

            // Map addresses to AddressResponse and categorize them
            foreach (var ia in addresses)
            {
                var address = new AddressResponse
                {
                    Institute_address_id = ia.Institute_address_id,
                    Institute_id = ia.Institute_id,
                    country_id = ia.country_id,
                    CountryName = ia.CountryName,
                    state_id = ia.state_id,
                    StateName = ia.StateName,
                    city_id = ia.city_id,
                    CityName = ia.CityName,
                    house = ia.house,
                    pincode = ia.pincode,
                    district_id = ia.district_id,
                    DistrictName = ia.DistrictName,
                    Locality = ia.Locality,
                    Landmark = ia.Landmark,
                    Mobile_number = ia.Mobile_number,
                    Email = ia.Email,
                    AddressType_id = ia.AddressType_id,
                    AddressTypeName = ia.AddressTypeName,
                    en_date = ia.en_date
                };

                // Categorize based on AddressType_id
                if (ia.AddressType_id == 3) // Billing Address
                {
                    addressResponse.BillingAddress.Add(address);
                }
                else if (ia.AddressType_id == 4) // Mailing Address
                {
                    addressResponse.MailingAddress.Add(address);
                }
            }

            return addressResponse;
        }
        private async Task<List<InstituteSMMappingResponse>> GetInstituteSMMappings(int instituteId)
        {
            string query = @"
        SELECT ism.SM_Mapping_Id, ism.Institute_id, ism.SM_Id, sm.Social_Media_Type AS SM_Name, ism.URL 
        FROM tbl_InstitueSMMapping ism
        JOIN tbl_SocialMediaMaster sm ON ism.SM_Id = sm.Social_Media_id
        WHERE ism.Institute_id = @Institute_id";

            var smMappings = await _connection.QueryAsync<InstituteSMMappingResponse>(query, new { Institute_id = instituteId });
            return smMappings.ToList();
        }
        private async Task<List<SchoolContactResponse>> GetSchoolContacts(int instituteId)
        {
            string query = @"
        SELECT sc.School_Contact_id, sc.ContactType_id, ct.ContactType AS ContactType_Name, 
               sc.Institute_id, sc.Contact_Person_name, sc.Telephone_number, sc.Email_ID, sc.Mobile_number, sc.isPrimary, sc.en_date 
        FROM tbl_SchoolContact sc 
        LEFT JOIN tbl_ContactType ct ON sc.ContactType_id = ct.ContactType_id 
        WHERE sc.Institute_id = @Institute_id";

            var schoolContacts = await _connection.QueryAsync<SchoolContactResponse>(query, new { Institute_id = instituteId });
            return schoolContacts.ToList();
        }
        private async Task<List<AcademicInfo>> GetAcademicInfos(int instituteId)
        {
            string query = "SELECT * FROM tbl_AcademicInfo WHERE Institute_id = @Institute_id";
            var academicInfos = await _connection.QueryAsync<AcademicInfo>(query, new { Institute_id = instituteId });
            foreach (var data in academicInfos)
            {
                data.StatusName = data.Status == true ? "Active" : "InActive";
            }
            return academicInfos.ToList();
        }
        private async Task<SemesterInfo> GetSemesterInfo(int instituteId)
        {
            string query = "SELECT * FROM tbl_SemesterInfo WHERE Institute_id = @Institute_id";
            var semesterInfo = await _connection.QueryFirstOrDefaultAsync<SemesterInfo>(query, new { Institute_id = instituteId });
            return semesterInfo ?? new SemesterInfo();
        }
        public async Task<ServiceResponse<List<AcademicYearMaster>>> GetAcademicYearList(int InstituteId)
        {
            // SQL query to fetch academic year info, assuming we're working with datetime strings
            var query = @"
    SELECT 
        AcademicYearStartMonth, 
        AcademicYearEndMonth
    FROM 
        tbl_AcademicInfo
    WHERE 
        Institute_id = @InstituteId AND Status = 1";  // assuming Status = 1 means active

            try
            {
                // Execute the query
                var academicYears = await _connection.QueryAsync<dynamic>(query, new { InstituteId });

                // Check if data was retrieved
                if (academicYears.Any())
                {
                    // Map the result to the list of AcademicYearMaster objects
                    var academicYearList = academicYears.Select(ay => new AcademicYearMaster
                    {
                        InstituteId = InstituteId,

                        // Extract only the year from AcademicYearStartMonth and AcademicYearEndMonth
                        YearName = $"{DateTime.Parse(ay.AcademicYearStartMonth.ToString()).Year} - {DateTime.Parse(ay.AcademicYearEndMonth.ToString()).Year}"
                    }).ToList();

                    return new ServiceResponse<List<AcademicYearMaster>>(true, "Academic year list retrieved successfully.", academicYearList, 200);
                }
                else
                {
                    // No records found for the given institute
                    return new ServiceResponse<List<AcademicYearMaster>>(false, "No academic years found for the specified institute.", new List<AcademicYearMaster>(), 404);
                }
            }
            catch (Exception ex)
            {
                // Return an error response
                return new ServiceResponse<List<AcademicYearMaster>>(false, $"An error occurred: {ex.Message}", new List<AcademicYearMaster>(), 500);
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
                INSERT INTO tbl_InstituteAddress (Institute_id, country_id, state_id, city_id, house, pincode, district_id, Mobile_number, Email, en_date, AddressType_id)
                VALUES (@Institute_id, @country_id, @state_id, @city_id, @house, @pincode, @district_id, @Mobile_number, @Email, @en_date, @AddressType_id)";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"
                INSERT INTO tbl_InstituteAddress (Institute_id, country_id, state_id, city_id, house, pincode, district_id, Mobile_number, Email, en_date, AddressType_id)
                VALUES (@Institute_id, @country_id, @state_id, @city_id, @house, @pincode, @district_id, @Mobile_number, @Email, @en_date, @AddressType_id)";

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
                    // Set the Institute ID
                    data.Institute_id = InstitutionId;

                    // Ensure that the day is set to the 1st for both AcademicYearStartMonth and AcademicYearEndMonth
                    data.AcademicYearStartMonth = new DateTime(data.AcademicYearStartMonth.Year, data.AcademicYearStartMonth.Month, 1);
                    data.AcademicYearEndMonth = new DateTime(data.AcademicYearEndMonth.Year, data.AcademicYearEndMonth.Month, 1);

                    // Generate AcaInfoYearCode (since it's not part of the request body)
                    string startMonthName = data.AcademicYearStartMonth.ToString("MMM").ToUpper();  // Get the first 3 letters of the month
                    int startYear = data.AcademicYearStartMonth.Year;
                    data.AcaInfoYearCode = $"{data.Institute_id}{startMonthName}{startYear}";  // e.g., 3JUL2024
                }
            }

            // Check if records exist for the institution
            string query = "SELECT COUNT(*) FROM [tbl_AcademicInfo] WHERE Institute_id = @InstituteId";
            int count = await _connection.ExecuteScalarAsync<int>(query, new { InstituteId = InstitutionId });

            if (count > 0)
            {
                // Delete existing records for the institution
                string deleteQuery = "DELETE FROM [tbl_AcademicInfo] WHERE Institute_id = @InstituteId";
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { InstituteId = InstitutionId });

                if (rowsAffected > 0)
                {
                    // Insert new academic information after deletion
                    string insertQuery = @"
            INSERT INTO [tbl_AcademicInfo] 
            (Institute_id, [AcademicYearStartMonth], [AcademicYearEndMonth], [AcaInfoYearCode], Status)
            VALUES (@Institute_id, @AcademicYearStartMonth, @AcademicYearEndMonth, @AcaInfoYearCode, @Status)";

                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                // Insert directly if no records exist
                string insertQuery = @"
        INSERT INTO [tbl_AcademicInfo] 
        (Institute_id, [AcademicYearStartMonth], [AcademicYearEndMonth], [AcaInfoYearCode], Status)
        VALUES (@Institute_id, @AcademicYearStartMonth, @AcademicYearEndMonth, @AcaInfoYearCode, @Status)";

                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }

            return addedRecords;
        }
        //private async Task<int> AddUpdateAcademicInfo(List<AcademicInfo> request, int InstitutionId)
        //{
        //    int addedRecords = 0;
        //    if (request != null)
        //    {
        //        foreach (var data in request)
        //        {
        //            data.Institute_id = InstitutionId;
        //        }
        //    }
        //    string query = "SELECT COUNT(*) FROM [tbl_AcademicInfo] WHERE Institute_id = @InstituteId";
        //    int count = await _connection.ExecuteScalarAsync<int>(query, new { InstituteId = InstitutionId });
        //    if (count > 0)
        //    {
        //        string deleteQuery = "DELETE FROM [tbl_AcademicInfo] WHERE Institute_id = @InstituteId";
        //        int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { InstituteId = InstitutionId });
        //        if (rowsAffected > 0)
        //        {
        //            string insertQuery = @"
        //        INSERT INTO [tbl_AcademicInfo] (Institute_id, [AcademicYearStartMonth], [AcademicYearEndMonth], Status)
        //        VALUES (@Institute_id, @AcademicYearStartMonth, @AcademicYearEndMonth, @Status)";
        //            // Execute the query with multiple parameterized sets of values
        //            addedRecords = await _connection.ExecuteAsync(insertQuery, request);
        //        }
        //    }
        //    else
        //    {
        //        string insertQuery = @"
        //        INSERT INTO [tbl_AcademicInfo] (Institute_id, [AcademicYearStartMonth], [AcademicYearEndMonth], Status)
        //        VALUES (@Institute_id, @AcademicYearStartMonth, @AcademicYearEndMonth, @Status)";
        //        // Execute the query with multiple parameterized sets of values
        //        addedRecords = await _connection.ExecuteAsync(insertQuery, request);
        //    }
        //    return addedRecords;
        //}
        private async Task<int> AddUpdateSemesterInfo(SemesterInfo request, int InstitutionId)
        {
            int rowsAffected = 0;

            // Set day to the 1st of the month for both SemesterStartDate and SemesterEndDate
            if (request.SemesterStartDate.HasValue)
            {
                request.SemesterStartDate = new DateTime(request.SemesterStartDate.Value.Year, request.SemesterStartDate.Value.Month, 1);
            }

            if (request.SemesterEndDate.HasValue)
            {
                request.SemesterEndDate = new DateTime(request.SemesterEndDate.Value.Year, request.SemesterEndDate.Value.Month, 1);
            }

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

        //    private async Task<int> AddUpdateSemesterInfo(SemesterInfo request, int InstitutionId)
        //    {
        //        int rowsAffected = 0;
        //        if (request.SemesterInfoId == 0)
        //        {
        //            request.Institute_id = InstitutionId;

        //            var insertQuery = @"
        //            INSERT INTO tbl_SemesterInfo (Institute_id, IsSemester, SemesterStartDate, SemesterEndDate)
        //            VALUES (@Institute_id, @IsSemester, @SemesterStartDate, @SemesterEndDate);";
        //            // Execute the query with parameterized values
        //            rowsAffected = await _connection.ExecuteAsync(insertQuery, request);
        //        }
        //        else
        //        {
        //            var updateQuery = @"
        //UPDATE tbl_SemesterInfo
        //SET Institute_id = @Institute_id,
        //    IsSemester = @IsSemester,
        //    SemesterStartDate = @SemesterStartDate,
        //    SemesterEndDate = @SemesterEndDate
        //WHERE SemesterInfoId = @SemesterInfoId;";

        //            rowsAffected = await _connection.ExecuteAsync(updateQuery, request);
        //        }
        //        return rowsAffected;
        //    }
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