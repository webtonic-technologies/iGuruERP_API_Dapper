using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IOptionalFeeRepository
    {
        Task<int> AddUpdateOptionalFee(AddUpdateOptionalFeeRequest request);
        Task<IEnumerable<OptionalFeeResponse>> GetAllOptionalFees(GetAllOptionalFeesRequest request);
        Task<OptionalFeeResponse> GetOptionalFeeById(int optionalFeeID);
        Task<int> UpdateOptionalFeeStatus(int optionalFeeID);
    }
}
