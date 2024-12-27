using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using SiteAdmin_API.DTOs.Responses;

namespace SiteAdmin_API.Repository.Implementations
{
    public class ChairmanRepository : IChairmanRepository
    {
        private readonly IDbConnection _dbConnection;

        public ChairmanRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public async Task<ServiceResponse<string>> AddUpdateChairman(AddUpdateChairmanRequest request)
        {
            try
            {
                int chairmanID;

                // Insert or update the chairman in tblInstituteChairman
                if (request.ChairmanID == 0)
                {
                    string query = @"INSERT INTO tblInstituteChairman (Name, MobileNumber, EmailID, UserName, Password) 
                             VALUES (@Name, @MobileNumber, @EmailID, @UserName, @Password);
                             SELECT CAST(SCOPE_IDENTITY() as int);";  // Retrieve the newly inserted ChairmanID

                    chairmanID = await _dbConnection.ExecuteScalarAsync<int>(query, new
                    {
                        request.Name,
                        request.MobileNumber,
                        request.EmailID,
                        request.UserName,
                        request.Password
                    });
                }
                else
                {
                    string query = @"UPDATE tblInstituteChairman 
                             SET Name = @Name, MobileNumber = @MobileNumber, EmailID = @EmailID, 
                                 UserName = @UserName, Password = @Password 
                             WHERE ChairmanID = @ChairmanID";

                    int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                    {
                        request.ChairmanID,
                        request.Name,
                        request.MobileNumber,
                        request.EmailID,
                        request.UserName,
                        request.Password
                    });

                    chairmanID = request.ChairmanID;  // Use the existing ChairmanID if updating
                }

                // Check if chairman insert/update was successful
                if (chairmanID > 0)
                {
                    // Delete existing associations for this ChairmanID
                    string deleteAssociationsQuery = @"DELETE FROM tblInstituteChairmanAssociation WHERE ChairmanID = @ChairmanID";
                    await _dbConnection.ExecuteAsync(deleteAssociationsQuery, new { ChairmanID = chairmanID });

                    // Insert the new associations into tblInstituteChairmanAssociation
                    foreach (var institute in request.Institutes)
                    {
                        string associationQuery = @"INSERT INTO tblInstituteChairmanAssociation (ChairmanID, InstituteID) 
                                            VALUES (@ChairmanID, @InstituteID)";

                        await _dbConnection.ExecuteAsync(associationQuery, new
                        {
                            ChairmanID = chairmanID,  // Use the correct ChairmanID
                            InstituteID = institute.InstituteID
                        });
                    }

                    return new ServiceResponse<string>(true, "Chairman added/updated successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to add/update chairman.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetAllChairmanResponse>>> GetAllChairman()
        {
            var query = @"
                SELECT c.Name, c.MobileNumber, c.EmailID, c.UserName, c.Password,
                    i.InstituteOnboardID AS InstituteID, i.InstituteOnboardName AS InstituteName
                FROM tblInstituteChairman c
                LEFT JOIN tblInstituteChairmanAssociation ca ON c.ChairmanID = ca.ChairmanID
                LEFT JOIN tblInstituteOnboard i ON ca.InstituteID = i.InstituteOnboardID";

            var chairmen = await _dbConnection.QueryAsync<GetAllChairmanResponse, InstituteResponse, GetAllChairmanResponse>(
                query,
                (chairman, institute) =>
                {
                    chairman.Institutes = chairman.Institutes ?? new List<InstituteResponse>();
                    chairman.Institutes.Add(institute);
                    return chairman;
                },
                splitOn: "InstituteID");

            return new ServiceResponse<IEnumerable<GetAllChairmanResponse>>(true, "Chairmen retrieved successfully", chairmen, 200);
        }


        public async Task<ServiceResponse<string>> DeleteChairman(int chairmanID)
        {
            try
            {
                string query = @"UPDATE tblInstituteChairman
                                 SET IsActive = 0
                                 WHERE ChairmanID = @ChairmanID";

                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { ChairmanID = chairmanID });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Chairman soft-deleted successfully.", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to soft-delete chairman.", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }
    }
}
