using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Implementations
{
    public class FeeGroupService : IFeeGroupService
    {
        private readonly IFeeGroupRepository _feeGroupRepository;

        public FeeGroupService(IFeeGroupRepository feeGroupRepository)
        {
            _feeGroupRepository = feeGroupRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateFeeGroup(AddUpdateFeeGroupRequest request)
        {
            var result = await _feeGroupRepository.AddUpdateFeeGroup(request);
            if (result > 0)
            {
                return new ServiceResponse<int>(true, "FeeGroup processed successfully", result, 200);
            }
            return new ServiceResponse<int>(false, "Failed to process FeeGroup", result, 400);
        }

        public async Task<ServiceResponse<IEnumerable<FeeGroupResponse>>> GetAllFeeGroups(GetAllFeeGroupRequest request)
        {
            var feeGroups = await _feeGroupRepository.GetAllFeeGroups(request);
            return new ServiceResponse<IEnumerable<FeeGroupResponse>>(true, "FeeGroups retrieved successfully", feeGroups, 200, feeGroups.Count());
        }

        public async Task<ServiceResponse<FeeGroupResponse>> GetFeeGroupById(int feeGroupID)
        {
            var feeGroup = await _feeGroupRepository.GetFeeGroupById(feeGroupID);
            if (feeGroup != null)
            {
                return new ServiceResponse<FeeGroupResponse>(true, "FeeGroup retrieved successfully", feeGroup, 200);
            }
            return new ServiceResponse<FeeGroupResponse>(false, "FeeGroup not found", null, 404);
        }

        public async Task<ServiceResponse<bool>> UpdateFeeGroupStatus(int feeGroupID)
        {
            try
            {
                int rowsAffected = await _feeGroupRepository.UpdateFeeGroupStatus(feeGroupID);

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "FeeGroup status updated successfully", true, 200);
                }

                return new ServiceResponse<bool>(false, "Failed to update FeeGroup status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
