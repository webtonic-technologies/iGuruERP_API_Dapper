using Admission_API.DTOs.Requests;
using Admission_API.DTOs.Response;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Repository.Interfaces
{
    public interface IEnquiryRepository
    {
        Task<ServiceResponse<string>> AddEnquiry(List<EnquiryRequest> requests, int leadStageID, int InstituteID);
        Task<ServiceResponse<List<GetAllEnquiryResponse>>> GetAllEnquiries(GetAllRequest request);
        Task<ServiceResponse<List<GetEnquiryListResponse>>> GetEnquiryList(GetEnqueryListRequest request);
        Task<ServiceResponse<List<GetLeadInformationResponse>>> GetLeadInformation(GetLeadInformationRequest request); 
        Task<ServiceResponse<string>> SendEnquiryMessage(SendEnquiryMessageRequest request);
        Task<ServiceResponse<List<EnquirySMS>>> GetSMSReport();
        Task<ServiceResponse<string>> AddLeadComment(AddLeadCommentRequest request);

    }
}




 