using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Admission_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Implementations
{
    public class EnquirySetupService : IEnquirySetupService
    {
        private readonly IEnquirySetupRepository _repository;

        public EnquirySetupService(IEnquirySetupRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddUpdateEnquirySetup(EnquirySetup request)
        {
            return await _repository.AddUpdateEnquirySetup(request);
        }

        public async Task<ServiceResponse<List<EnquirySetup>>> GetAllEnquirySetups(GetAllRequest request)
        {
            return await _repository.GetAllEnquirySetups(request);
        }

        public async Task<ServiceResponse<bool>> DeleteEnquirySetup(int enquirySetupID)
        {
            return await _repository.DeleteEnquirySetup(enquirySetupID);
        }
    }
}
