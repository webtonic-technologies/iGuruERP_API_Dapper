using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Interfaces
{
    public interface ILateFeeService
    {
        Task<ServiceResponse<int>> AddUpdateLateFee(AddUpdateLateFeeRequest request);
        Task<ServiceResponse<IEnumerable<LateFeeResponse>>> GetAllLateFee(GetAllLateFeeRequest request);
        Task<ServiceResponse<LateFeeResponse>> GetLateFeeById(int lateFeeID);
        Task<ServiceResponse<int>> UpdateLateFeeStatus(int lateFeeID);  // Ensure this is the correct return type
    }
}
