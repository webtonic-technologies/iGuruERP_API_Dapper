using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface ICautionDepositService
    {
        Task<ServiceResponse<int>> AddUpdateCautionDeposit(AddUpdateCautionDepositRequest request);
        Task<ServiceResponse<PagedResponse<CautionDepositResponse>>> GetAllCautionDeposits(GetAllCautionDepositRequest request);
        Task<ServiceResponse<CautionDepositResponse>> GetCautionDepositById(int cautionFeeId);
        Task<ServiceResponse<bool>> DeleteCautionDeposit(int cautionFeeId);
    }
}
