using Admission_API.DTOs.Requests;
using Admission_API.DTOs.Response;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Interfaces
{
    public interface IEnquirySetupService
    {

        Task<ServiceResponse<string>> AddUpdateEnquirySetup(EnquirySetup request, List<EnquiryOption> options);
        Task<ServiceResponse<List<EnquirySetupResponse>>> GetAllEnquirySetups(GetAllRequest request);
        Task<ServiceResponse<bool>> DeleteEnquirySetup(int enquirySetupID);
        Task<ServiceResponse<bool>> FormStatus(int EnquirySetupID);
        Task<ServiceResponse<bool>> MandatoryStatus(int EnquirySetupID);

    }
}
