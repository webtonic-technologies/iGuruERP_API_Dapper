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
    public class EnquirySetupService : IEnquirySetupService
    {
        private readonly IEnquirySetupRepository _repository;

        public EnquirySetupService(IEnquirySetupRepository repository)
        {
            _repository = repository;
        }


        public async Task<ServiceResponse<string>> AddUpdateEnquirySetup(EnquirySetup request, List<EnquiryOption> options)
        {
            return await _repository.AddUpdateEnquirySetup(request, options);
        }


        public async Task<ServiceResponse<List<EnquirySetupResponse>>> GetAllEnquirySetups(GetAllRequest request)
        {
            return await _repository.GetAllEnquirySetups(request);
        }

        public async Task<ServiceResponse<bool>> DeleteEnquirySetup(int enquirySetupID)
        {
            return await _repository.DeleteEnquirySetup(enquirySetupID);
        }

        public async Task<ServiceResponse<bool>> FormStatus(int EnquirySetupID)
        {
            return await _repository.FormStatus(EnquirySetupID);
        }
        public async Task<ServiceResponse<bool>> MandatoryStatus(int EnquirySetupID)
        {
            return await _repository.MandatoryStatus(EnquirySetupID);
        }
    }
}
