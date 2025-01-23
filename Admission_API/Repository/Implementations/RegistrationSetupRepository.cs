using Admission_API.DTOs.Requests;
using Admission_API.DTOs.Response;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Admission_API.Repository.Implementations
{
    public class RegistrationSetupRepository : IRegistrationSetupRepository
    {
        private readonly IDbConnection _dbConnection;

        public RegistrationSetupRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
         
        public async Task<ServiceResponse<string>> AddUpdateRegistrationSetup(RegistrationSetup request, List<RegistrationOption> options)
        {
            // SQL query for inserting or updating EnquirySetup
            var query = request.RegistrationSetupID == 0 ?
                @"INSERT INTO tblRegistrationSetup (FieldName, FieldTypeID, IsMultiChoice, InstituteID) 
                  VALUES (@FieldName, @FieldTypeID, @IsMultiChoice, @InstituteID); 
                  SELECT SCOPE_IDENTITY();" :
                                @"UPDATE tblRegistrationSetup 
                  SET FieldName = @FieldName, FieldTypeID = @FieldTypeID, IsMultiChoice = @IsMultiChoice, InstituteID = @InstituteID 
                  WHERE RegistrationSetupID = @RegistrationSetupID";

            // Parameters for EnquirySetup query
            var parameters = new
            {
                RegistrationSetupID = request.RegistrationSetupID,
                FieldName = request.FieldName,
                FieldTypeID = request.FieldTypeID,
                IsMultiChoice = request.IsMultiChoice,
                InstituteID = request.InstituteID
            };

            // Execute the EnquirySetup query and get the result (EnquirySetupID for inserts)
            var result = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);

            // If IsMultiChoice is true, update options in tblEnquiryOptions
            if (request.IsMultiChoice)
            {
                // Delete existing options for the current EnquirySetupID
                var deleteQuery = @"DELETE FROM tblRegistrationOptions  WHERE RegistrationSetupID  = @RegistrationSetupID";
                await _dbConnection.ExecuteAsync(deleteQuery, new { RegistrationSetupID = result == 0 ? request.RegistrationSetupID : result });

                // Insert new options
                foreach (var option in options)
                {
                    var optionQuery = @"
                    INSERT INTO tblRegistrationOptions (RegistrationSetupID, FieldTypeID, Options) 
                    VALUES (@RegistrationSetupID, @FieldTypeID, @Options)";

                    var optionParameters = new
                    {
                        RegistrationSetupID = result == 0 ? request.RegistrationSetupID : result, // Use the result from SCOPE_IDENTITY() if it's a new record
                        FieldTypeID = option.FieldTypeID,
                        Options = option.Options
                    };

                    // Execute the query for each option
                    await _dbConnection.ExecuteAsync(optionQuery, optionParameters);
                }
            }

            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }
         
        public async Task<ServiceResponse<List<RegistrationSetupResponse>>> GetAllRegistrationSetups(GetAllRequest request)
        {
            // SQL query to fetch EnquirySetup and associated EnquiryOptions, filtering by InstituteID
            var query = @"
            SELECT es.RegistrationSetupID, es.FieldName, es.FieldTypeID, ef.FieldType, es.IsMultiChoice, es.IsInForm, es.IsMandatory, es.IsDeleted
            FROM tblRegistrationSetup es
            INNER JOIN tblEnquiryFieldType ef ON es.FieldTypeID = ef.FieldTypeID
            WHERE es.InstituteID = @InstituteID AND es.IsDeleted = 0
            ORDER BY es.RegistrationSetupID 
            OFFSET @PageNumber ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            // Execute the main query to get EnquirySetups, filtered by InstituteID
            var result = (await _dbConnection.QueryAsync<RegistrationSetupResponse>(query, new
            {
                PageNumber = (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                request.InstituteID // Pass InstituteID to the query
            })).ToList();

            // SQL query to fetch options for each EnquirySetup (filtered by InstituteID)
            var optionsQuery = @"
            SELECT eo.RegistrationSetupID, eo.OptionID, eo.Options
            FROM tblRegistrationOptions eo
            INNER JOIN tblRegistrationSetup es ON eo.RegistrationSetupID = es.RegistrationSetupID
            WHERE es.InstituteID = @InstituteID"; // Filter by InstituteID

            // Get options for all enquiry setups
            var options = await _dbConnection.QueryAsync<RegistrationOptionResponse>(optionsQuery, new { request.InstituteID });

            // Group options by EnquirySetupID
            var optionsGrouped = options.GroupBy(o => o.RegistrationSetupID).ToDictionary(g => g.Key, g => g.ToList());

            // Populate Options in each EnquirySetupResponse
            foreach (var enquirySetup in result)
            {
                if (optionsGrouped.ContainsKey(enquirySetup.RegistrationSetupID))
                {
                    enquirySetup.Options = optionsGrouped[enquirySetup.RegistrationSetupID];
                }
                else
                {
                    enquirySetup.Options = new List<RegistrationOptionResponse>(); // No options found for this EnquirySetupID
                }
            }

            // Get the total count of EnquirySetups for the given InstituteID
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM tblRegistrationSetup WHERE InstituteID = @InstituteID AND IsDeleted = 0",
                new { request.InstituteID });

            return new ServiceResponse<List<RegistrationSetupResponse>>(true, "Operation Successful", result, 200, totalCount);
        }
          
        public async Task<ServiceResponse<bool>> DeleteRegistrationSetup(int registrationSetupID)
        {
            var query = "Update tblRegistrationSetup Set IsDeleted = 1 WHERE RegistrationSetupID = @RegistrationSetupID";
            var result = await _dbConnection.ExecuteAsync(query, new { RegistrationSetupID = registrationSetupID });
            return new ServiceResponse<bool>(true, "Registration Field Deleted Successfully", result > 0, 200);
        }

        public async Task<ServiceResponse<bool>> FormStatus(int registrationSetupID)
        {
            var query = "UPDATE tblRegistrationSetup SET IsInForm = ~IsInForm WHERE RegistrationSetupID = @RegistrationSetupID";
            var result = await _dbConnection.ExecuteAsync(query, new { RegistrationSetupID = registrationSetupID });
            return new ServiceResponse<bool>(true, "Form Field Status Updated Successfully", result > 0, 200);
        }

        public async Task<ServiceResponse<bool>> MandatoryStatus(int registrationSetupID)
        {
            var query = "UPDATE tblRegistrationSetup SET IsMandatory = ~IsMandatory WHERE RegistrationSetupID = @RegistrationSetupID";
            var result = await _dbConnection.ExecuteAsync(query, new { RegistrationSetupID = registrationSetupID });
            return new ServiceResponse<bool>(true, "Mandatory Field Status Updated Successfully", result > 0, 200);
        }
    }
}
