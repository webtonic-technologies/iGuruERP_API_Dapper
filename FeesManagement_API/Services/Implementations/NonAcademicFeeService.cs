using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Implementations
{
    public class NonAcademicFeeService : INonAcademicFeeService
    {
        private readonly INonAcademicFeeRepository _repository;

        public NonAcademicFeeService(INonAcademicFeeRepository repository)
        {
            _repository = repository;
        }

        public ServiceResponse<string> AddNonAcademicFee(AddUpdateNonAcademicFeeRequest request)
        {
            var result = _repository.AddNonAcademicFee(request);
            return new ServiceResponse<string>(true, "Successfully added/updated the non-academic fee", result, 200);
        }

        public ServiceResponse<List<GetNonAcademicFeeResponse>> GetNonAcademicFee(GetNonAcademicFeeRequest request)
        {
            var result = _repository.GetNonAcademicFee(request);
            return new ServiceResponse<List<GetNonAcademicFeeResponse>>(true, "Successfully retrieved data", result, 200);
        }

        public ServiceResponse<string> DeleteNonAcademicFee(int nonAcademicFeesID)
        {
            var result = _repository.DeleteNonAcademicFee(nonAcademicFeesID);
            return new ServiceResponse<string>(true, "Successfully deleted the non-academic fee", result, 200);
        }
    }
}
