using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Implementations
{
    public class LateFeeService : ILateFeeService
    {
        private readonly ILateFeeRepository _lateFeeRepository;

        public LateFeeService(ILateFeeRepository lateFeeRepository)
        {
            _lateFeeRepository = lateFeeRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateLateFee(AddUpdateLateFeeRequest request)
        {
            var result = await _lateFeeRepository.AddUpdateLateFee(request);
            return new ServiceResponse<int>(true, "Late Fee added/updated successfully", result, 200);
        }

        public async Task<ServiceResponse<IEnumerable<LateFeeResponse>>> GetAllLateFee(GetAllLateFeeRequest request)
        {
            // Call the repository method
            var serviceResponse = await _lateFeeRepository.GetAllLateFee(request);

            // Return the ServiceResponse with the data
            return new ServiceResponse<IEnumerable<LateFeeResponse>>(
                serviceResponse.Success,
                serviceResponse.Message,
                serviceResponse.Data,  // Extract Data from the ServiceResponse
                serviceResponse.StatusCode,
                serviceResponse.TotalCount);
        }

        //public async Task<ServiceResponse<IEnumerable<LateFeeResponse>>> GetAllLateFee(GetAllLateFeeRequest request)
        //{
        //    var lateFees = await _lateFeeRepository.GetAllLateFee(request);
        //    return new ServiceResponse<IEnumerable<LateFeeResponse>>(true, "Late Fees retrieved successfully", lateFees, 200);
        //}

        public async Task<ServiceResponse<LateFeeResponse>> GetLateFeeById(int lateFeeID)
        {
            var lateFee = await _lateFeeRepository.GetLateFeeById(lateFeeID);
            if (lateFee != null)
            {
                return new ServiceResponse<LateFeeResponse>(true, "Late Fee retrieved successfully", lateFee, 200);
            }
            return new ServiceResponse<LateFeeResponse>(false, "Late Fee not found", null, 404);
        }

        public async Task<ServiceResponse<int>> UpdateLateFeeStatus(int lateFeeID)  // This method should match the return type
        {
            var result = await _lateFeeRepository.UpdateLateFeeStatus(lateFeeID);
            if (result > 0)
            {
                return new ServiceResponse<int>(true, "Late Fee status updated successfully", result, 200);
            }
            return new ServiceResponse<int>(false, "Failed to update Late Fee status", 0, 400);
        }

        public async Task<ServiceResponse<IEnumerable<FeeTenureResponse>>> GetFeeTenureDDL(GetFeeTenureDDLRequest request)
        {
            var feeTenureResponses = await _lateFeeRepository.GetFeeTenureDDL(request);
            return new ServiceResponse<IEnumerable<FeeTenureResponse>>(true, "Fee Tenures retrieved successfully", feeTenureResponses, 200);
        }

    }
}
