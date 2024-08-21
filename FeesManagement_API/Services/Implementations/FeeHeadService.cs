using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Implementations
{
    public class FeeHeadService : IFeeHeadService
    {
        private readonly IFeeHeadRepository _feeHeadRepository;

        public FeeHeadService(IFeeHeadRepository feeHeadRepository)
        {
            _feeHeadRepository = feeHeadRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateFeeHead(AddUpdateFeeHeadRequest request)
        {
            var rowsAffected = await _feeHeadRepository.AddUpdateFeeHead(request);
            if (rowsAffected > 0)
            {
                return new ServiceResponse<int>(true, "FeeHeads processed successfully", rowsAffected, 200);
            }
            return new ServiceResponse<int>(false, "Failed to process FeeHeads", rowsAffected, 400);
        }

        public async Task<ServiceResponse<IEnumerable<FeeHeadResponse>>> GetAllFeeHead(GetAllFeeHeadRequest request)
        {
            var feeHeads = await _feeHeadRepository.GetAllFeeHead(request);
            return new ServiceResponse<IEnumerable<FeeHeadResponse>>(true, "FeeHeads retrieved successfully", feeHeads, 200);
        }

        public async Task<ServiceResponse<FeeHeadResponse>> GetFeeHeadById(int feeHeadId)
        {
            var feeHead = await _feeHeadRepository.GetFeeHeadById(feeHeadId);
            if (feeHead != null)
            {
                return new ServiceResponse<FeeHeadResponse>(true, "FeeHead retrieved successfully", feeHead, 200);
            }
            return new ServiceResponse<FeeHeadResponse>(false, "FeeHead not found", null, 404);
        }

        public async Task<ServiceResponse<int>> DeleteFeeHead(int feeHeadId)
        {
            var rowsAffected = await _feeHeadRepository.DeleteFeeHead(feeHeadId);
            if (rowsAffected > 0)
            {
                return new ServiceResponse<int>(true, "FeeHead deleted successfully", rowsAffected, 200);
            }
            return new ServiceResponse<int>(false, "Failed to delete FeeHead", rowsAffected, 400);
        }
    }
}
