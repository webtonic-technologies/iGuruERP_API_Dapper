using Admission_API.DTOs.Requests;
using Admission_API.DTOs.Response;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Admission_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Implementations
{
    public class EnquiryService : IEnquiryService
    {
        private readonly IEnquiryRepository _repository;

        public EnquiryService(IEnquiryRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddEnquiry(List<EnquiryRequest> requests, int leadStageID, int InstituteID)
        {
            return await _repository.AddEnquiry(requests, leadStageID, InstituteID);
        }

        public async Task<ServiceResponse<List<GetAllEnquiryResponse>>> GetAllEnquiries(GetAllRequest request)
        {
            return await _repository.GetAllEnquiries(request);
        } 
        public async Task<ServiceResponse<List<GetEnquiryListResponse>>> GetEnquiryList(GetEnqueryListRequest request)
        {
            return await _repository.GetEnquiryList(request);
        }
        public async Task<ServiceResponse<List<GetLeadInformationResponse>>> GetLeadInformation(GetLeadInformationRequest request)
        {
            return await _repository.GetLeadInformation(request);
        }

        public async Task<ServiceResponse<string>> SendEnquiryMessage(SendEnquiryMessageRequest request)
        {
            return await _repository.SendEnquiryMessage(request);
        }

        public async Task<ServiceResponse<List<EnquirySMS>>> GetSMSReport()
        {
            return await _repository.GetSMSReport();
        }

        public async Task<ServiceResponse<string>> AddLeadComment(AddLeadCommentRequest request)
        {
            return await _repository.AddLeadComment(request);
        }

    }
}
