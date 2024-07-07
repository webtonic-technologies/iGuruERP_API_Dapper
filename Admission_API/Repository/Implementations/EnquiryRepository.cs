using Admission_API.DTOs.Requests;
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

        public async Task<ServiceResponse<string>> AddEnquiry(Enquiry request)
        {
            var query = @"INSERT INTO tblEnquiryMaster (EnquiryGroupID, FieldTypeID, FieldTypeValue, LeadStageID, FollowupDate, Comments) 
                          VALUES (@EnquiryGroupID, @FieldTypeID, @FieldTypeValue, @LeadStageID, @FollowupDate, @Comments)";
            var parameters = new
            {
                request.EnquiryGroupID,
                request.FieldTypeID,
                request.FieldTypeValue,
                request.LeadStageID,
                request.FollowupDate,
                request.Comments
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }

        public async Task<ServiceResponse<List<Enquiry>>> GetAllEnquiries(GetAllRequest request)
        {
            var query = @"SELECT EnquiryID, EnquiryGroupID, FieldTypeID, FieldTypeValue, LeadStageID, FollowupDate, Comments 
                          FROM tblEnquiryMaster 
                          ORDER BY EnquiryID 
                          OFFSET @PageNumber ROWS 
                          FETCH NEXT @PageSize ROWS ONLY";
            var result = (await _dbConnection.QueryAsync<Enquiry>(query, new { PageNumber = (request.PageNumber - 1) * request.PageSize, request.PageSize })).ToList();
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblEnquiryMaster");
            return new ServiceResponse<List<Enquiry>>(true, "Operation Successful", result, 200, totalCount);
        }

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
    }
}
