using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Repository.Interfaces
{
    public interface IEnquiryRepository
    {
        Task<ServiceResponse<string>> AddEnquiry(Enquiry request);
        Task<ServiceResponse<List<Enquiry>>> GetAllEnquiries(GetAllRequest request);
        Task<ServiceResponse<string>> SendEnquiryMessage(SendEnquiryMessageRequest request);
        Task<ServiceResponse<List<EnquirySMS>>> GetSMSReport();
    }
}
