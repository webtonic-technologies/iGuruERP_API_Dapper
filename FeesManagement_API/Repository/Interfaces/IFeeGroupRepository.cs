using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IFeeGroupRepository
    {
        Task<int> AddUpdateFeeGroups(AddUpdateFeeGroupsRequest request);
        Task<IEnumerable<FeeGroupResponse>> GetAllFeeGroups(GetAllFeeGroupRequest request);
        Task<FeeGroupResponse> GetFeeGroupById(int feeGroupID);
        Task<int> UpdateFeeGroupStatus(int feeGroupID, string reason);
    }
}
