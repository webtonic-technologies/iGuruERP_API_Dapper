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
    public class EnquirySetupRepository : IEnquirySetupRepository
    {
        private readonly IDbConnection _dbConnection;

        public EnquirySetupRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateEnquirySetup(EnquirySetup request, List<EnquiryOption> options)
        {
            // SQL query for inserting or updating EnquirySetup
            var query = request.EnquirySetupID == 0 ?
                @"INSERT INTO tblEnquirySetup (FieldName, FieldTypeID, IsMultiChoice, InstituteID) 
          VALUES (@FieldName, @FieldTypeID, @IsMultiChoice, @InstituteID); 
          SELECT SCOPE_IDENTITY();" :
                @"UPDATE tblEnquirySetup 
          SET FieldName = @FieldName, FieldTypeID = @FieldTypeID, IsMultiChoice = @IsMultiChoice, InstituteID = @InstituteID 
          WHERE EnquirySetupID = @EnquirySetupID";

            // Parameters for EnquirySetup query
            var parameters = new
            {
                EnquirySetupID = request.EnquirySetupID,
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
                var deleteQuery = @"DELETE FROM tblEnquiryOptions WHERE EnquirySetupID = @EnquirySetupID";
                await _dbConnection.ExecuteAsync(deleteQuery, new { EnquirySetupID = result == 0 ? request.EnquirySetupID : result });

                // Insert new options
                foreach (var option in options)
                {
                    var optionQuery = @"
                    INSERT INTO tblEnquiryOptions (EnquirySetupID, FieldTypeID, Options) 
                    VALUES (@EnquirySetupID, @FieldTypeID, @Options)";

                    var optionParameters = new
                    {
                        EnquirySetupID = result == 0 ? request.EnquirySetupID : result, // Use the result from SCOPE_IDENTITY() if it's a new record
                        FieldTypeID = option.FieldTypeID,
                        Options = option.Options
                    };

                    // Execute the query for each option
                    await _dbConnection.ExecuteAsync(optionQuery, optionParameters);
                }
            }

            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }


        //public async Task<ServiceResponse<string>> AddUpdateEnquirySetup(EnquirySetup request, List<EnquiryOption> options)
        //{
        //    // SQL query for inserting or updating EnquirySetup
        //    var query = request.EnquirySetupID == 0 ?
        //        @"INSERT INTO tblEnquirySetup (FieldName, FieldTypeID, IsMultiChoice, InstituteID) 
        //  VALUES (@FieldName, @FieldTypeID, @IsMultiChoice, @InstituteID); 
        //  SELECT SCOPE_IDENTITY();" :
        //        @"UPDATE tblEnquirySetup 
        //  SET FieldName = @FieldName, FieldTypeID = @FieldTypeID, IsMultiChoice = @IsMultiChoice, InstituteID = @InstituteID 
        //  WHERE EnquirySetupID = @EnquirySetupID";

        //    // Parameters for EnquirySetup query
        //    var parameters = new
        //    {
        //        EnquirySetupID = request.EnquirySetupID,
        //        FieldName = request.FieldName,
        //        FieldTypeID = request.FieldTypeID,
        //        IsMultiChoice = request.IsMultiChoice,
        //        InstituteID = request.InstituteID
        //    };

        //    // Execute the EnquirySetup query and get the result (EnquirySetupID for inserts)
        //    var result = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);

        //    // If IsMultiChoice is true, insert options into tblEnquiryOptions
        //    if (request.IsMultiChoice)
        //    {
        //        foreach (var option in options)
        //        {
        //            var optionQuery = @"
        //        INSERT INTO tblEnquiryOptions (EnquirySetupID, FieldTypeID, Options) 
        //        VALUES (@EnquirySetupID, @FieldTypeID, @Options)";

        //            var optionParameters = new
        //            {
        //                EnquirySetupID = request.EnquirySetupID == 0 ? result : request.EnquirySetupID, // Use the result from SCOPE_IDENTITY() if it's a new record
        //                FieldTypeID = option.FieldTypeID,
        //                Options = option.Options
        //            };

        //            // Execute the query for each option
        //            await _dbConnection.ExecuteAsync(optionQuery, optionParameters);
        //        }
        //    }

        //    return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        //}

        public async Task<ServiceResponse<List<EnquirySetupResponse>>> GetAllEnquirySetups(GetAllRequest request)
        {
            // SQL query to fetch EnquirySetup and associated EnquiryOptions, filtering by InstituteID
            var query = @"
            SELECT es.EnquirySetupID, es.FieldName, es.FieldTypeID, ef.FieldType, es.IsMultiChoice, es.IsInForm, es.IsMandatory, es.IsDefault, es.IsDeleted
            FROM tblEnquirySetup es
            INNER JOIN tblEnquiryFieldType ef ON es.FieldTypeID = ef.FieldTypeID
            WHERE es.InstituteID = @InstituteID AND es.IsDeleted = 0
            ORDER BY es.EnquirySetupID 
            OFFSET @PageNumber ROWS 
            FETCH NEXT @PageSize ROWS ONLY";

            // Execute the main query to get EnquirySetups, filtered by InstituteID
            var result = (await _dbConnection.QueryAsync<EnquirySetupResponse>(query, new
            {
                PageNumber = (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                request.InstituteID // Pass InstituteID to the query
            })).ToList();

            // SQL query to fetch options for each EnquirySetup (filtered by InstituteID)
            var optionsQuery = @"
            SELECT eo.EnquirySetupID, eo.OptionID, eo.Options
            FROM tblEnquiryOptions eo
            INNER JOIN tblEnquirySetup es ON eo.EnquirySetupID = es.EnquirySetupID
            WHERE es.InstituteID = @InstituteID"; // Filter by InstituteID

            // Get options for all enquiry setups
            var options = await _dbConnection.QueryAsync<EnquiryOptionResponse>(optionsQuery, new { request.InstituteID });

            // Group options by EnquirySetupID
            var optionsGrouped = options.GroupBy(o => o.EnquirySetupID).ToDictionary(g => g.Key, g => g.ToList());

            // Populate Options in each EnquirySetupResponse
            foreach (var enquirySetup in result)
            {
                if (optionsGrouped.ContainsKey(enquirySetup.EnquirySetupID))
                {
                    enquirySetup.Options = optionsGrouped[enquirySetup.EnquirySetupID];
                }
                else
                {
                    enquirySetup.Options = new List<EnquiryOptionResponse>(); // No options found for this EnquirySetupID
                }
            }

            // Get the total count of EnquirySetups for the given InstituteID
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM tblEnquirySetup WHERE InstituteID = @InstituteID AND IsDeleted = 0",
                new { request.InstituteID });

            return new ServiceResponse<List<EnquirySetupResponse>>(true, "Operation Successful", result, 200, totalCount);
        }

        //public async Task<ServiceResponse<List<EnquirySetupResponse>>> GetAllEnquirySetups(GetAllRequest request)
        //{
        //    // SQL query to fetch EnquirySetup and associated EnquiryOptions
        //    var query = @"
        //    SELECT es.EnquirySetupID, es.FieldName, es.FieldTypeID, ef.FieldType, es.IsMultiChoice, es.IsInForm, es.IsMandatory, es.IsDefault, es.IsDeleted
        //    FROM tblEnquirySetup es
        //    INNER JOIN tblEnquiryFieldType ef ON es.FieldTypeID = ef.FieldTypeID
        //    ORDER BY es.EnquirySetupID 
        //    OFFSET @PageNumber ROWS 
        //    FETCH NEXT @PageSize ROWS ONLY";

        //    // Execute the main query to get EnquirySetups
        //    var result = (await _dbConnection.QueryAsync<EnquirySetupResponse>(query, new { PageNumber = (request.PageNumber - 1) * request.PageSize, request.PageSize })).ToList();

        //    // Fetch options for each EnquirySetup (if any)
        //    var optionsQuery = @"
        //    SELECT eo.EnquirySetupID, eo.OptionID, eo.Options
        //    FROM tblEnquiryOptions eo";

        //    // Get options for all enquiry setups
        //    var options = await _dbConnection.QueryAsync<EnquiryOptionResponse>(optionsQuery);

        //    // Group options by EnquirySetupID
        //    var optionsGrouped = options.GroupBy(o => o.EnquirySetupID).ToDictionary(g => g.Key, g => g.ToList());

        //    // Populate Options in each EnquirySetupResponse
        //    foreach (var enquirySetup in result)
        //    {
        //        if (optionsGrouped.ContainsKey(enquirySetup.EnquirySetupID))
        //        {
        //            enquirySetup.Options = optionsGrouped[enquirySetup.EnquirySetupID];
        //        }
        //        else
        //        {
        //            enquirySetup.Options = new List<EnquiryOptionResponse>(); // No options found for this EnquirySetupID
        //        }
        //    }

        //    // Get the total count of EnquirySetups
        //    var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblEnquirySetup");

        //    return new ServiceResponse<List<EnquirySetupResponse>>(true, "Operation Successful", result, 200, totalCount);
        //}


        //public async Task<ServiceResponse<List<EnquirySetupResponse>>> GetAllEnquirySetups(GetAllRequest request)
        //{
        //    var query = @"SELECT EnquirySetupID, FieldName, FieldTypeID, IsDefault, IsDeleted 
        //                  FROM tblEnquirySetup 
        //                  ORDER BY EnquirySetupID 
        //                  OFFSET @PageNumber ROWS 
        //                  FETCH NEXT @PageSize ROWS ONLY";
        //    var result = (await _dbConnection.QueryAsync<EnquirySetupResponse>(query, new { PageNumber = (request.PageNumber - 1) * request.PageSize, request.PageSize })).ToList();
        //    var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblEnquirySetup");
        //    return new ServiceResponse<List<EnquirySetupResponse>>(true, "Operation Successful", result, 200, totalCount);
        //}

        public async Task<ServiceResponse<bool>> DeleteEnquirySetup(int enquirySetupID)
        {
            var query = "Update tblEnquirySetup Set IsDeleted = 1 WHERE EnquirySetupID = @EnquirySetupID";
            var result = await _dbConnection.ExecuteAsync(query, new { EnquirySetupID = enquirySetupID });
            return new ServiceResponse<bool>(true, "Enquiry Field Deleted Successfully", result > 0, 200);
        }
         
        public async Task<ServiceResponse<bool>> FormStatus(int EnquirySetupID)
        {
            var query = "UPDATE tblEnquirySetup SET IsInForm = ~IsInForm WHERE EnquirySetupID = @EnquirySetupID";
            var result = await _dbConnection.ExecuteAsync(query, new { EnquirySetupID = EnquirySetupID });
            return new ServiceResponse<bool>(true, "Form Field Status Updated Successfully", result > 0, 200);
        }

        public async Task<ServiceResponse<bool>> MandatoryStatus(int EnquirySetupID)
        {
            var query = "UPDATE tblEnquirySetup SET IsMandatory = ~IsMandatory WHERE EnquirySetupID = @EnquirySetupID";
            var result = await _dbConnection.ExecuteAsync(query, new { EnquirySetupID = EnquirySetupID });
            return new ServiceResponse<bool>(true, "Mandatory Field Status Updated Successfully", result > 0, 200);
        }
    }
}
