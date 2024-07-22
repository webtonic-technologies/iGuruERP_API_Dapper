using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;

namespace Institute_API.Repository.Implementations
{
    public class InstituteAffiliationRepository : IInstituteAffiliationRepository
    {
        private readonly IDbConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public InstituteAffiliationRepository(IDbConnection connection, IWebHostEnvironment hostingEnvironment)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<ServiceResponse<int>> AddUpdateInstituteAffiliation(AffiliationDTO request)
        {
            try
            {
                if (request.Affiliation_info_id == 0)
                {
                    string sql = @"INSERT INTO [tbl_AffiliationInfo] (Institute_id, AffiliationBoardLogo, 
                       AffiliationBoardName, AffiliationNumber, AffiliationCertificateNumber, InstituteCode, en_date)
                       VALUES (@Institute_id, @AffiliationBoardLogo, @AffiliationBoardName, 
                       @AffiliationNumber, @AffiliationCertificateNumber, @InstituteCode, @en_date);
                       SELECT SCOPE_IDENTITY();"; // Retrieve the inserted id

                    InstituteAffiliation affiliation = new()
                    {
                        Institute_id = request.Institute_id,
                        AffiliationBoardLogo = ImageUpload(request.AffiliationBoardLogo),
                        AffiliationBoardName = request.AffiliationBoardName,
                        AffiliationNumber = request.AffiliationNumber,
                        AffiliationCertificateNumber = request.AffiliationCertificateNumber,
                        InstituteCode = request.InstituteCode,
                        en_date = request.en_date
                    };
                    // Execute the query and retrieve the inserted id
                    int insertedId = await _connection.ExecuteScalarAsync<int>(sql, affiliation);
                    if (insertedId > 0 || request.Accreditations != null)
                    {
                        int accred = await AddUpdateAccreditation(request.Accreditations ??= ([]), insertedId);
                        if (accred > 0)
                        {
                            return new ServiceResponse<int>(true, "Affiliation added successfully", insertedId, 200);
                        }
                        else
                        {
                            return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                        }
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }
                else
                {
                    string sql = @"UPDATE [tbl_AffiliationInfo]
                      SET Institute_id = @Institute_id,
                          AffiliationBoardLogo = @AffiliationBoardLogo,
                          AffiliationBoardName = @AffiliationBoardName,
                          AffiliationNumber = @AffiliationNumber,
                          AffiliationCertificateNumber = @AffiliationCertificateNumber,
                          InstituteCode = @InstituteCode,
                          en_date = @en_date
                      WHERE Affiliation_info_id = @AffiliationInfoId";

                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, new
                    {
                        request.Institute_id,
                        AffiliationBoardLogo = ImageUpload(request.AffiliationBoardLogo),
                        request.AffiliationBoardName,
                        request.AffiliationNumber,
                        request.AffiliationCertificateNumber,
                        request.InstituteCode,
                        AffiliationInfoId = request.Affiliation_info_id,
                        request.en_date
                    });
                    if (affectedRows > 0 || request.Accreditations != null)
                    {
                        int accred = await AddUpdateAccreditation(request.Accreditations ??= ([]), request.Affiliation_info_id);
                        if (accred > 0)
                        {
                            return new ServiceResponse<int>(true, "Affiliation Updated successfully", request.Affiliation_info_id, 200);
                        }
                        else
                        {
                            return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                        }
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
        public async Task<ServiceResponse<AffiliationDTO>> GetAffiliationInfoById(int Id)
        {
            try
            {
                var response = new AffiliationDTO();
                string sql = @"SELECT *
                       FROM [dbo].[tbl_AffiliationInfo]
                       WHERE [Institute_id] = @Id";

                // Execute the query and retrieve the result
                var affiliation = await _connection.QueryFirstOrDefaultAsync<InstituteAffiliation>(sql, new { Id });
                if (affiliation != null)
                {
                    string selectQuery = @"SELECT Accreditation_id, Affiliation_id, Accreditation_Number
                       FROM [dbo].[tbl_Accreditation]
                       WHERE Affiliation_id = @AffiliationId";

                    // Execute the query and retrieve the result
                    var accreditations = await _connection.QueryAsync<Accreditation>(selectQuery, new { AffiliationId = affiliation.Affiliation_info_id });
                    response.Affiliation_info_id = affiliation.Affiliation_info_id;
                    response.AffiliationNumber = affiliation.AffiliationNumber;
                    response.AffiliationCertificateNumber = affiliation.AffiliationCertificateNumber;
                    response.AffiliationBoardName = affiliation.AffiliationBoardName;
                    response.Accreditations = accreditations != null ? accreditations.AsList() : [];
                    response.AffiliationBoardLogo = GetImage(affiliation.AffiliationBoardLogo);
                    response.en_date = affiliation.en_date;
                    response.Institute_id = affiliation.Institute_id;
                    response.InstituteCode = affiliation.InstituteCode;

                    return new ServiceResponse<AffiliationDTO>(true, "Record found", response, 200);
                }
                else
                {
                    return new ServiceResponse<AffiliationDTO>(false, "Record not found", new AffiliationDTO(), 500);
                }
            }
            catch(Exception ex)
            {
                return new ServiceResponse<AffiliationDTO>(false, ex.Message, new AffiliationDTO(), 500);
            }
          
        }
        private async Task<int> AddUpdateAccreditation(List<Accreditation> request, int Affiliationid)
        {
            int addedRecords = 0;
            if (request != null)
            {
                foreach (var data in request)
                {
                    data.Affiliation_id = Affiliationid;
                }
            }
            string query = "SELECT COUNT(*) FROM tbl_Accreditation WHERE Affiliation_id = @Affiliation_id";
            int count = await _connection.ExecuteScalarAsync<int>(query, new { Affiliation_id = Affiliationid });
            if (count > 0)
            {
                string deleteQuery = "DELETE FROM tbl_Accreditation WHERE Affiliation_id = @Affiliation_id";
                int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { Affiliation_id = Affiliationid });
                if (rowsAffected > 0)
                {
                    string insertQuery = @"INSERT INTO tbl_Accreditation (Affiliation_id, Accreditation_Number)
                       VALUES (@Affiliation_id, @Accreditation_Number);";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"INSERT INTO tbl_Accreditation (Affiliation_id, Accreditation_Number)
                       VALUES (@Affiliation_id, @Accreditation_Number);";
                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }
        private string ImageUpload(string image)
        {
            if (string.IsNullOrEmpty(image) || image == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "InstituteAffiliation");

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
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "InstituteAffiliation", Filename);

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
