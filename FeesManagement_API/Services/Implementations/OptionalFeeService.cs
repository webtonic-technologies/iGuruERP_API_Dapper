using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class OptionalFeeService : IOptionalFeeService
    {
        private readonly IOptionalFeeRepository _optionalFeeRepository;

        public OptionalFeeService(IOptionalFeeRepository optionalFeeRepository)
        {
            _optionalFeeRepository = optionalFeeRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateOptionalFee(AddUpdateOptionalFeeRequest request)
        {
            var result = await _optionalFeeRepository.AddUpdateOptionalFee(request);
            return new ServiceResponse<int>(true, "Optional Fee(s) updated successfully.", result, 200);
        }

        public async Task<ServiceResponse<IEnumerable<OptionalFeeResponse>>> GetAllOptionalFees(GetAllOptionalFeesRequest request)
        {
            var result = await _optionalFeeRepository.GetAllOptionalFees(request);
            return new ServiceResponse<IEnumerable<OptionalFeeResponse>>(true, "Data retrieved successfully", result, 200);
        }

        public async Task<ServiceResponse<OptionalFeeResponse>> GetOptionalFeeById(int optionalFeeID)
        {
            var result = await _optionalFeeRepository.GetOptionalFeeById(optionalFeeID);
            return new ServiceResponse<OptionalFeeResponse>(true, "Optional Fee retrieved successfully.", result, 200);
        }

        public async Task<int> UpdateOptionalFeeStatus(int optionalFeeID)
        {
            return await _optionalFeeRepository.UpdateOptionalFeeStatus(optionalFeeID);
        }
    }
}
