using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class CautionDepositService : ICautionDepositService
    {
        private readonly ICautionDepositRepository _cautionDepositRepository;

        public CautionDepositService(ICautionDepositRepository cautionDepositRepository)
        {
            _cautionDepositRepository = cautionDepositRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateCautionDeposit(AddUpdateCautionDepositRequest request)
        {
            var cautionFeeId = await _cautionDepositRepository.AddUpdateCautionDeposit(request);
            return new ServiceResponse<int>(true, "Caution deposit added/updated successfully", cautionFeeId, 200);
        }

        public async Task<ServiceResponse<PagedResponse<CautionDepositResponse>>> GetAllCautionDeposits(GetAllCautionDepositRequest request)
        {
            var cautionDeposits = await _cautionDepositRepository.GetAllCautionDeposits(request);
            return new ServiceResponse<PagedResponse<CautionDepositResponse>>(true, "Caution deposits retrieved successfully", cautionDeposits, 200);
        }

        public async Task<ServiceResponse<CautionDepositResponse>> GetCautionDepositById(int cautionFeeId)
        {
            var cautionDeposit = await _cautionDepositRepository.GetCautionDepositById(cautionFeeId);
            if (cautionDeposit == null)
            {
                return new ServiceResponse<CautionDepositResponse>(false, "Caution deposit not found", null, 404);
            }
            return new ServiceResponse<CautionDepositResponse>(true, "Caution deposit retrieved successfully", cautionDeposit, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteCautionDeposit(int cautionFeeId)
        {
            var result = await _cautionDepositRepository.DeleteCautionDeposit(cautionFeeId);
            return new ServiceResponse<bool>(result > 0, result > 0 ? "Caution deposit deleted successfully" : "Caution deposit not found", result > 0, result > 0 ? 200 : 404);
        }
    }
}
