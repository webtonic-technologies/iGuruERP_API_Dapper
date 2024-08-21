using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IOptionalFeeService
    {
        Task<ServiceResponse<int>> AddUpdateOptionalFee(AddUpdateOptionalFeeRequest request);
        Task<ServiceResponse<IEnumerable<OptionalFeeResponse>>> GetAllOptionalFees(GetAllOptionalFeesRequest request);
        Task<ServiceResponse<OptionalFeeResponse>> GetOptionalFeeById(int optionalFeeID);
        Task<int> UpdateOptionalFeeStatus(int optionalFeeID);
    }
}
