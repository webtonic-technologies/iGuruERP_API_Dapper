using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface ICautionDepositRepository
    {
        Task<int> AddUpdateCautionDeposit(AddUpdateCautionDepositRequest request);
        Task<PagedResponse<CautionDepositResponse>> GetAllCautionDeposits(GetAllCautionDepositRequest request);
        Task<CautionDepositResponse> GetCautionDepositById(int cautionFeeId);
        Task<int> DeleteCautionDeposit(int cautionFeeId);
    }
}
