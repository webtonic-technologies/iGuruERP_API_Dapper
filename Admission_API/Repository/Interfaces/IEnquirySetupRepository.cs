using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Repository.Interfaces
{
    public interface IEnquirySetupRepository
    {
        Task<ServiceResponse<string>> AddUpdateEnquirySetup(EnquirySetup request);
        Task<ServiceResponse<List<EnquirySetup>>> GetAllEnquirySetups(GetAllRequest request);
        Task<ServiceResponse<bool>> DeleteEnquirySetup(int enquirySetupID);
    }
}
