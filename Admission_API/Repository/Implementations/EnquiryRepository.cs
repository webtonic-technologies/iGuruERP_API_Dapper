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
    public class EnquiryRepository : IEnquiryRepository
    {
        private readonly IDbConnection _dbConnection;

        public EnquiryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddEnquiry(List<EnquiryRequest> requests, int leadStageID, int InstituteID)
        {
            var enquiryIDs = new List<int>();  // Store EnquiryIDs of the inserted records

            // Auto-generate a single LeadCode for the entire batch of records
            string leadCode = "L" + DateTime.Now.Ticks.ToString(); // Example: generating LeadCode based on timestamp

            // Loop through the requests and insert each one into tblEnquiryMaster
            foreach (var request in requests)
            {
                // Insert into tblEnquiryMaster
                var enquiryMasterQuery = @"
                INSERT INTO tblEnquiryMaster (EnquirySetupID, FieldTypeID, FieldTypeValue, IsMultiChoice, OptionID, LeadCode)
                VALUES (@EnquirySetupID, @FieldTypeID, @FieldTypeValue, @IsMultiChoice, @OptionID, @LeadCode);
                SELECT SCOPE_IDENTITY();"; // Retrieve the EnquiryID for the inserted row

                var enquiryMasterParameters = new
                {
                    request.EnquirySetupID,
                    request.FieldTypeID,
                    request.FieldTypeValue,
                    request.IsMultiChoice,
                    request.OptionID,
                    LeadCode = leadCode // Use the same LeadCode for all records
                };

                var enquiryID = await _dbConnection.ExecuteScalarAsync<int>(enquiryMasterQuery, enquiryMasterParameters);
                enquiryIDs.Add(enquiryID); // Add EnquiryID to the list
            }

            // Insert into tblEnquiryLead once, using the same LeadCode
            var enquiryLeadQuery = @"
            INSERT INTO tblEnquiryLead (LeadCode, LeadStageID, InstituteID)
            VALUES (@LeadCode, @LeadStageID, @InstituteID);
            SELECT SCOPE_IDENTITY();"; // Optionally retrieve the LeadID if needed

            var enquiryLeadParameters = new
            {
                LeadCode = leadCode, // Use the same LeadCode for all records
                LeadStageID = leadStageID,
                InstituteID = InstituteID
            };

            var leadID = await _dbConnection.ExecuteScalarAsync<int>(enquiryLeadQuery, enquiryLeadParameters);

            return new ServiceResponse<string>(true, "Operation Successful", string.Join(",", enquiryIDs), 200);
        }


        public async Task<ServiceResponse<List<GetAllEnquiryResponse>>> GetAllEnquiries(GetAllRequest request)
        {
            // SQL query to fetch lead information and associated EnquiryInfo in one result set
            var query = @"
            SELECT 
                el.LeadID, 
                el.LeadCode, 
                el.LeadStageID, 
                lsm.LeadStage,
                em.EnquirySetupID, 
                es.FieldName, 
                em.FieldTypeValue, 
                eo.Options
            FROM tblEnquiryMaster em
            LEFT JOIN tblEnquiryOptions eo ON em.OptionID = eo.OptionID
            LEFT JOIN tblEnquirySetup es ON em.EnquirySetupID = es.EnquirySetupID
            LEFT JOIN tblEnquiryLead el ON em.LeadCode = el.LeadCode
            LEFT JOIN tblLeadStageMaster lsm ON el.LeadStageID = lsm.LeadStageID
            WHERE el.InstituteID = @InstituteID
            ORDER BY el.LeadCode, em.EnquirySetupID";

            // Fetch the results and manually group them into the GetAllEnquiryResponse format
            var result = new List<GetAllEnquiryResponse>();

            var queryResult = await _dbConnection.QueryAsync<dynamic>(query, new { InstituteID = request.InstituteID });

            // Manually group the results by LeadID and add EnquiryInfo
            foreach (var lead in queryResult)
            {
                var leadResponse = result.FirstOrDefault(l => l.LeadID == lead.LeadID);
                if (leadResponse == null)
                {
                    leadResponse = new GetAllEnquiryResponse
                    {
                        LeadID = lead.LeadID,
                        LeadCode = lead.LeadCode,
                        LeadStageID = lead.LeadStageID,
                        LeadStage = lead.LeadStage,
                        EnquiryInfo = new List<GetEnquiry>()
                    };
                    result.Add(leadResponse);
                }

                leadResponse.EnquiryInfo.Add(new GetEnquiry
                {
                    EnquirySetupID = lead.EnquirySetupID,
                    FieldName = lead.FieldName,
                    FieldTypeValue = lead.FieldTypeValue,
                    Options = lead.Options
                });
            }

            // Get the total count of distinct leads matching the criteria
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(@"
            SELECT COUNT(DISTINCT em.LeadCode)
            FROM tblEnquiryMaster em
            LEFT JOIN tblEnquiryLead el ON em.LeadCode = el.LeadCode
            WHERE em.InstituteID = @InstituteID",
                new { InstituteID = request.InstituteID });

            return new ServiceResponse<List<GetAllEnquiryResponse>>(true, "Operation Successful", result, 200, totalCount);
        }


        public async Task<ServiceResponse<List<GetEnquiryListResponse>>> GetEnquiryList(GetEnqueryListRequest request)
        {
            // SQL query to fetch lead information and associated EnquiryInfo
            var query = @"
    SELECT 
        el.LeadID, 
        el.LeadCode, 
        el.LeadStageID, 
        lsm.LeadStage,
        em.EnquirySetupID, 
        es.FieldName, 
        em.FieldTypeValue, 
        eo.Options
    FROM tblEnquiryMaster em
    LEFT JOIN tblEnquiryOptions eo ON em.OptionID = eo.OptionID
    LEFT JOIN tblEnquirySetup es ON em.EnquirySetupID = es.EnquirySetupID
    LEFT JOIN tblEnquiryLead el ON em.LeadCode = el.LeadCode
    LEFT JOIN tblLeadStageMaster lsm ON el.LeadStageID = lsm.LeadStageID
    WHERE el.InstituteID = @InstituteID
    ORDER BY el.LeadStageID, el.LeadCode, em.EnquirySetupID";

            // Fetch the results and group them by LeadStage
            var result = new List<GetEnquiryListResponse>();

            var queryResult = await _dbConnection.QueryAsync<dynamic>(query, new { InstituteID = request.InstituteID });

            // Group the results by LeadStage
            foreach (var lead in queryResult)
            {
                // Find the LeadStage group or create a new one if it doesn't exist
                var leadStageResponse = result.FirstOrDefault(l => l.LeadStage == lead.LeadStage);
                if (leadStageResponse == null)
                {
                    leadStageResponse = new GetEnquiryListResponse
                    {
                        LeadStage = lead.LeadStage,
                        Leads = new List<GetLeadDetails>()
                    };
                    result.Add(leadStageResponse);
                }

                // Find or create the lead details inside the LeadStage group
                var leadResponse = leadStageResponse.Leads.FirstOrDefault(l => l.LeadID == lead.LeadID);
                if (leadResponse == null)
                {
                    leadResponse = new GetLeadDetails
                    {
                        LeadID = lead.LeadID,
                        LeadCode = lead.LeadCode,
                        LeadStageID = lead.LeadStageID,
                        LeadStage = lead.LeadStage,
                        EnquiryInfo = new List<GetEnquiryList>()
                    };
                    leadStageResponse.Leads.Add(leadResponse);
                }

                // Add the enquiry details to the lead's EnquiryInfo list
                leadResponse.EnquiryInfo.Add(new GetEnquiryList
                {
                    EnquirySetupID = lead.EnquirySetupID,
                    FieldName = lead.FieldName,
                    FieldTypeValue = lead.FieldTypeValue,
                    Options = lead.Options
                });
            }

            // Get the total count of distinct leads matching the criteria
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(@"
            SELECT COUNT(DISTINCT em.LeadCode)
            FROM tblEnquiryMaster em
            LEFT JOIN tblEnquiryLead el ON em.LeadCode = el.LeadCode
            WHERE em.InstituteID = @InstituteID",
                        new { InstituteID = request.InstituteID });

            return new ServiceResponse<List<GetEnquiryListResponse>>(true, "Operation Successful", result, 200, totalCount);
        }

        public async Task<ServiceResponse<List<GetLeadInformationResponse>>> GetLeadInformation(GetLeadInformationRequest request)
        {
            // SQL query to fetch lead information, associated EnquiryInfo, and lead comments in one result set
            var query = @"
            SELECT 
                el.LeadID, 
                el.LeadCode, 
                el.LeadStageID, 
                lsm.LeadStage,
                em.EnquirySetupID, 
                es.FieldName, 
                em.FieldTypeValue, 
                eo.Options,
                lf.FollowupDate,
                lf.Comments AS LeadComment,
                lf.LeadStageID AS FollowupLeadStageID
            FROM tblEnquiryMaster em
            LEFT JOIN tblEnquiryOptions eo ON em.OptionID = eo.OptionID
            LEFT JOIN tblEnquirySetup es ON em.EnquirySetupID = es.EnquirySetupID
            LEFT JOIN tblEnquiryLead el ON em.LeadCode = el.LeadCode
            LEFT JOIN tblLeadStageMaster lsm ON el.LeadStageID = lsm.LeadStageID
            LEFT JOIN tblEnquiryLeadFollowup lf ON el.LeadID = lf.LeadID
            WHERE el.InstituteID = @InstituteID AND el.LeadID = @LeadID";

            // Fetch the results
            var result = new List<GetLeadInformationResponse>();

            var queryResult = await _dbConnection.QueryAsync<dynamic>(query, new { InstituteID = request.InstituteID, LeadID = request.LeadID });

            // Manually group the results by LeadID and add EnquiryInfo and LeadComments
            foreach (var lead in queryResult)
            {
                var leadResponse = result.FirstOrDefault(l => l.LeadID == lead.LeadID);
                if (leadResponse == null)
                {
                    leadResponse = new GetLeadInformationResponse
                    {
                        LeadID = lead.LeadID,
                        LeadCode = lead.LeadCode,
                        LeadStageID = lead.LeadStageID,
                        LeadStage = lead.LeadStage,
                        EnquiryInfo = new List<GetLeadEnquiry>(),
                        LeadComments = new List<GetLeadComments>()
                    };
                    result.Add(leadResponse);
                }

                // Add EnquiryInfo
                leadResponse.EnquiryInfo.Add(new GetLeadEnquiry
                {
                    EnquirySetupID = lead.EnquirySetupID,
                    FieldName = lead.FieldName,
                    FieldTypeValue = lead.FieldTypeValue,
                    Options = lead.Options
                });

                // Add LeadComments
                leadResponse.LeadComments.Add(new GetLeadComments
                {
                    LeadStageID = lead.FollowupLeadStageID,
                    LeadStage = lead.LeadStage, // Assuming LeadStage is the same
                    Comments = lead.LeadComment,
                    FollowupDate = lead.FollowupDate != null
                ? ((DateTime)lead.FollowupDate).ToString("dd-MM-yyyy") // If it's a DateTime object, format it as DD-MM-YYYY
                : null // Handle null case if FollowupDate is null
                });
            }

            return new ServiceResponse<List<GetLeadInformationResponse>>(true, "Operation Successful", result, 200, null);
        }


        //public async Task<ServiceResponse<List<GetLeadInformationResponse>>> GetLeadInformation(GetLeadInformationRequest request)
        //{
        //    // SQL query to fetch lead information and associated EnquiryInfo in one result set
        //    var query = @"
        //    SELECT 
        //        el.LeadID, 
        //        el.LeadCode, 
        //        el.LeadStageID, 
        //        lsm.LeadStage,
        //        em.EnquirySetupID, 
        //        es.FieldName, 
        //        em.FieldTypeValue, 
        //        eo.Options
        //    FROM tblEnquiryMaster em
        //    LEFT JOIN tblEnquiryOptions eo ON em.OptionID = eo.OptionID
        //    LEFT JOIN tblEnquirySetup es ON em.EnquirySetupID = es.EnquirySetupID
        //    LEFT JOIN tblEnquiryLead el ON em.LeadCode = el.LeadCode
        //    LEFT JOIN tblLeadStageMaster lsm ON el.LeadStageID = lsm.LeadStageID
        //    WHERE el.InstituteID = @InstituteID AND el.LeadID = @LeadID";

        //    // Fetch the results and manually group them into the GetAllEnquiryResponse format
        //    var result = new List<GetLeadInformationResponse>();

        //    var queryResult = await _dbConnection.QueryAsync<dynamic>(query, new { InstituteID = request.InstituteID, LeadID = request.LeadID });

        //    // Manually group the results by LeadID and add EnquiryInfo
        //    foreach (var lead in queryResult)
        //    {
        //        var leadResponse = result.FirstOrDefault(l => l.LeadID == lead.LeadID);
        //        if (leadResponse == null)
        //        {
        //            leadResponse = new GetLeadInformationResponse
        //            {
        //                LeadID = lead.LeadID,
        //                LeadCode = lead.LeadCode,
        //                LeadStageID = lead.LeadStageID,
        //                LeadStage = lead.LeadStage,
        //                EnquiryInfo = new List<GetLeadEnquiry>()
        //            };
        //            result.Add(leadResponse);
        //        }

        //        leadResponse.EnquiryInfo.Add(new GetLeadEnquiry
        //        {
        //            EnquirySetupID = lead.EnquirySetupID,
        //            FieldName = lead.FieldName,
        //            FieldTypeValue = lead.FieldTypeValue,
        //            Options = lead.Options
        //        });
        //    }


        //    return new ServiceResponse<List<GetLeadInformationResponse>>(true, "Operation Successful", result, 200, null);
        //}


        public async Task<ServiceResponse<string>> SendEnquiryMessage(SendEnquiryMessageRequest request)
        {
            var query = @"INSERT INTO tblEnquirySMS (EnquiryID, TemplateID, SMSDetails) 
                          VALUES (@EnquiryID, @TemplateID, @SMSDetails)";
            var parameters = new
            {
                request.EnquiryID,
                request.TemplateID,
                request.SMSDetails
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }

        public async Task<ServiceResponse<List<EnquirySMS>>> GetSMSReport()
        {
            var query = @"SELECT es.EnquirySMSID, es.EnquiryID, es.TemplateID, es.SMSDetails 
                          FROM tblEnquirySMS es 
                          JOIN tblEnquiryMaster em ON es.EnquiryID = em.EnquiryID";
            var result = (await _dbConnection.QueryAsync<EnquirySMS>(query)).ToList();
            return new ServiceResponse<List<EnquirySMS>>(true, "Operation Successful", result, 200);
        }
 

        public async Task<ServiceResponse<string>> AddLeadComment(AddLeadCommentRequest request)
        {
            // Ensure the connection is open
            if (_dbConnection.State != System.Data.ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert into tblEnquiryLeadFollowup
                    var insertQuery = @"
                    INSERT INTO tblEnquiryLeadFollowup (LeadID, FollowupDate, Comments, LeadStageID)
                    VALUES (@LeadID, @FollowupDate, @Comments, @LeadStageID)";

                    var parameters = new
                    {
                        request.LeadID,
                        FollowupDate = DateTime.ParseExact(request.FollowupDate, "dd-MM-yyyy", null), // Parse date
                        request.Comments,
                        request.LeadStageID
                    };

                    var result = await _dbConnection.ExecuteAsync(insertQuery, parameters, transaction);

                    // Update tblEnquiryLead with the new LeadStageID
                    var updateQuery = @"
                    UPDATE tblEnquiryLead
                    SET LeadStageID = @LeadStageID
                    WHERE LeadID = @LeadID";

                    var updateParameters = new
                    {
                        request.LeadID,
                        request.LeadStageID
                    };

                    await _dbConnection.ExecuteAsync(updateQuery, updateParameters, transaction);

                    // Commit the transaction
                    transaction.Commit();

                    return new ServiceResponse<string>(true, "Lead comment added and LeadStage updated successfully", result.ToString(), 200);
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of an error
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, "An error occurred: " + ex.Message, null, 500);
                }
                finally
                {
                    // Close the connection after the operation
                    if (_dbConnection.State == System.Data.ConnectionState.Open)
                    {
                        _dbConnection.Close();
                    }
                }
            }
        }


    }
}
