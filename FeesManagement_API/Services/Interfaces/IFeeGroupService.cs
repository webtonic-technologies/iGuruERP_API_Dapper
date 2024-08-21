using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IFeeGroupService
    {
        Task<ServiceResponse<int>> AddUpdateFeeGroup(AddUpdateFeeGroupRequest request);
        Task<ServiceResponse<IEnumerable<FeeGroupResponse>>> GetAllFeeGroups(GetAllFeeGroupRequest request);
        Task<ServiceResponse<FeeGroupResponse>> GetFeeGroupById(int feeGroupID);
        Task<ServiceResponse<bool>> UpdateFeeGroupStatus(int feeGroupID);
    }
}
