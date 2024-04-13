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
                    string sql = @"INSERT INTO tbl_InstituteAffiliation (Institute_id, AffiliationBoardLogo, 
                       AffiliationBoardName, AffiliationNumber, AffiliationCertificateNumber, InstituteCode)
                       VALUES (@InstituteId, @AffiliationBoardLogo, @AffiliationBoardName, 
                       @AffiliationNumber, @AffiliationCertificateNumber, @InstituteCode);
                       SELECT SCOPE_IDENTITY();"; // Retrieve the inserted id

                    InstituteAffiliation affiliation = new()
                    {
                        Institute_id = request.Institute_id,
                        AffiliationBoardLogo = string.Empty,
                        AffiliationBoardName = request.AffiliationBoardName,
                        AffiliationNumber = request.AffiliationNumber,
                        AffiliationCertificateNumber = request.AffiliationCertificateNumber,
                        InstituteCode = request.InstituteCode
                    };
                    // Execute the query and retrieve the inserted id
                    int insertedId = await _connection.ExecuteScalarAsync<int>(sql, affiliation);
                    if (insertedId > 0 || request.Accreditations != null)
                    {
                        int accred = await AddUpdateAccreditation(request.Accreditations ??= ([]), insertedId);
                        if (accred > 0)
                        {
                            return new ServiceResponse<int>(true, "Affiliation added successfully", insertedId, 500);
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
                    string sql = @"UPDATE tbl_InstituteAffiliation
                      SET Institute_id = @InstituteId,
                          AffiliationBoardLogo = @AffiliationBoardLogo,
                          AffiliationBoardName = @AffiliationBoardName,
                          AffiliationNumber = @AffiliationNumber,
                          AffiliationCertificateNumber = @AffiliationCertificateNumber,
                          InstituteCode = @InstituteCode
                      WHERE Affiliation_info_id = @AffiliationInfoId";

                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, new
                    {
                        InstituteId = request.Institute_id,
                        request.AffiliationBoardLogo,
                        request.AffiliationBoardName,
                        request.AffiliationNumber,
                        request.AffiliationCertificateNumber,
                        request.InstituteCode,
                        AffiliationInfoId = request.Affiliation_info_id
                    });
                    if (affectedRows > 0 || request.Accreditations != null)
                    {
                        int accred = await AddUpdateAccreditation(request.Accreditations ??= ([]), request.Affiliation_info_id);
                        if (accred > 0)
                        {
                            return new ServiceResponse<int>(true, "Affiliation added successfully", request.Affiliation_info_id, 500);
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

        public async Task<ServiceResponse<string>> AddUpdateLogo(AffiliationLogoDTO request)
        {
            try
            {
                var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var fileName = Path.GetFileNameWithoutExtension(request.AffiliationBoardLogo.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(request.AffiliationBoardLogo.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await request.AffiliationBoardLogo.CopyToAsync(fileStream);
                }

                string sql = @"UPDATE tbl_InstituteAffiliation
                      SET AffiliationBoardLogo = @AffiliationBoardLogo
                      WHERE Affiliation_info_id = @AffiliationInfoId";

                // Execute the query and retrieve the number of affected rows
                int affectedRows = await _connection.ExecuteAsync(sql, new
                {
                    AffiliationBoardLogo = fileName, // Store file path in the database
                    AffiliationInfoId = request.Affiliation_info_id
                });
                if (affectedRows > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successful", "Affiliation added successfully", 500);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Some error occured", string.Empty, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<AffiliationDTO>> GetAffiliationInfoById(int Id)
        {
            try
            {
                var response = new AffiliationDTO();
                string sql = @"SELECT Affiliation_info_id, Institute_id, 
                              AffiliationBoardName, AffiliationNumber, 
                              AffiliationCertificateNumber, InstituteCode
                       FROM [dbo].[tbl_InstituteAffiliation]
                       WHERE Affiliation_info_id = @Id";

                // Execute the query and retrieve the result
                var affiliation = await _connection.QueryFirstOrDefaultAsync<InstituteAffiliation>(sql, new { Id });
                if (affiliation != null)
                {
                    string selectQuery = @"SELECT Accreditation_id, Affiliation_id, Accreditation_Number
                       FROM [dbo].[tbl_Accreditation]
                       WHERE Affiliation_id = @AffiliationId";

                    // Execute the query and retrieve the result
                    var accreditations = await _connection.QueryAsync<Accreditation>(selectQuery, new { AffiliationId = Id });
                    response.Affiliation_info_id = affiliation.Affiliation_info_id;
                    response.AffiliationNumber = affiliation.AffiliationNumber;
                    response.AffiliationCertificateNumber = affiliation.AffiliationCertificateNumber;
                    response.AffiliationBoardName = affiliation.AffiliationBoardName;
                    response.Accreditations = accreditations != null ? accreditations.AsList() : [];

                    return new ServiceResponse<AffiliationDTO>(true, "Record found", response, 500);
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

        public async Task<ServiceResponse<byte[]>> GetAffiliationLogoById(int Id)
        {
            try
            {
                var data = await _connection.QueryFirstOrDefaultAsync<InstituteAffiliation>(
                   "SELECT AffiliationBoardLogo FROM tbl_AffiliationInfo WHERE " +
                   "Affiliation_info_id = @Affiliation_info_id",
                   new { Affiliation_info_id = Id }) ?? throw new Exception("record not found");
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution", data.AffiliationBoardLogo);

                if (!File.Exists(filePath))
                {
                    throw new Exception("File not found");
                }
                var fileBytes = await File.ReadAllBytesAsync(filePath);

                return new ServiceResponse<byte[]>(true, "Record Found", fileBytes, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
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
                       VALUES (@AffiliationId, @AccreditationNumber);";

                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, request);
                }
            }
            else
            {
                string insertQuery = @"INSERT INTO tbl_Accreditation (Affiliation_id, Accreditation_Number)
                       VALUES (@AffiliationId, @AccreditationNumber);";
                // Execute the query with multiple parameterized sets of values
                addedRecords = await _connection.ExecuteAsync(insertQuery, request);
            }
            return addedRecords;
        }

    }
}
