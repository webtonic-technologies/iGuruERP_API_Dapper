using Admission_API.DTOs.Requests;
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

        public async Task<ServiceResponse<string>> AddEnquiry(Enquiry request)
        {
            return await _repository.AddEnquiry(request);
        }

        public async Task<ServiceResponse<List<Enquiry>>> GetAllEnquiries(GetAllRequest request)
        {
            return await _repository.GetAllEnquiries(request);
        }

        public async Task<ServiceResponse<string>> SendEnquiryMessage(SendEnquiryMessageRequest request)
        {
            return await _repository.SendEnquiryMessage(request);
        }

        public async Task<ServiceResponse<List<EnquirySMS>>> GetSMSReport()
        {
            return await _repository.GetSMSReport();
        }
    }
}
