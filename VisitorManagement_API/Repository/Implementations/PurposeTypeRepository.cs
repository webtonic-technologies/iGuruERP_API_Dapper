using Dapper;
using System.Data;
using System.Data.SqlClient;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;

namespace VisitorManagement_API.Repository.Implementations
{
    public class PurposeTypeRepository : IPurposeTypeRepository
    {
        private readonly IDbConnection _dbConnection;

        public PurposeTypeRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdatePurposeType(PurposeType purposeType)
        {
            try
            {
                if (purposeType.PurposeID == 0)
                {
                    // Insert new purpose type with InstituteID
                    string query = @"INSERT INTO tblPurposeType (Purpose, Description, Status, InstituteID) 
                             VALUES (@Purpose, @Description, @Status, @InstituteID)";
                    purposeType.Status = true;
                    int insertedValue = await _dbConnection.ExecuteAsync(query, new
                    {
                        purposeType.Purpose,
                        purposeType.Description,
                        purposeType.Status,
                        purposeType.InstituteID  // Include InstituteID in the insert query
                    });
                    if (insertedValue > 0)
                    {
                        return new ServiceResponse<string>(true, "Purpose Type Added Successfully", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to Add Purpose Type", "Failure", 400);
                }
                else
                {
                    // Update existing purpose type with InstituteID
                    string query = @"UPDATE tblPurposeType 
                             SET Purpose = @Purpose, Description = @Description, 
                                 Status = @Status, InstituteID = @InstituteID 
                             WHERE PurposeID = @PurposeID";
                    int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                    {
                        purposeType.Purpose,
                        purposeType.Description,
                        purposeType.PurposeID,
                        purposeType.Status,
                        purposeType.InstituteID  // Include InstituteID in the update query
                    });
                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<string>(true, "Purpose Type Updated Successfully", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to Update Purpose Type", "Failure", 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<PurposeType>>> GetAllPurposeTypes(GetAllPurposeTypeRequest request)
        {
            try
            {
                // Update the query to filter by InstituteID
                string query = "SELECT * FROM tblPurposeType WHERE Status = 1 AND InstituteID = @InstituteID";

                // Fetch the purpose types based on InstituteID
                var purposeTypes = await _dbConnection.QueryAsync<PurposeType>(query, new { InstituteID = request.InstituteID });

                // Apply pagination
                var paginatedPurposeTypes = purposeTypes.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

                return new ServiceResponse<IEnumerable<PurposeType>>(true, "Purpose Types Retrieved Successfully", paginatedPurposeTypes, 200, purposeTypes.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<PurposeType>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<PurposeType>> GetPurposeTypeById(int purposeTypeId)
        {
            try
            {
                string query = "SELECT * FROM tblPurposeType WHERE PurposeID = @PurposeID";
                var purposeType = await _dbConnection.QueryFirstOrDefaultAsync<PurposeType>(query, new { PurposeID = purposeTypeId });
                if (purposeType != null)
                {
                    return new ServiceResponse<PurposeType>(true, "Purpose Type Retrieved Successfully", purposeType, 200);
                }
                return new ServiceResponse<PurposeType>(false, "Purpose Type Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PurposeType>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePurposeTypeStatus(int purposeTypeId)
        {
            try
            {
                // Assuming there is a Status column in tblPurposeType table
                string query = "UPDATE tblPurposeType SET Status = CASE WHEN Status = 1 THEN 0 ELSE 1 END WHERE PurposeID = @PurposeID";
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { PurposeID = purposeTypeId });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Purpose Type Status Updated Successfully", true, 200);
                }
                return new ServiceResponse<bool>(false, "Failed to Update Purpose Type Status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
