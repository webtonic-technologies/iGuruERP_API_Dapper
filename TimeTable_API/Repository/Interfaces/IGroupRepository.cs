using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Models;
using TimeTable_API.DTOs.Responses;

namespace TimeTable_API.Repository.Interfaces
{
    public interface IGroupRepository
    {
        Task<ServiceResponse<string>> AddUpdateGroup(AddUpdateGroupRequest request);
        Task<ServiceResponse<List<GroupResponse>>> GetAllGroups(GetAllGroupsRequest request);
        Task<ServiceResponse<GroupResponse>> GetGroupById(int groupId);
        Task<ServiceResponse<bool>> DeleteGroup(int groupId);
    }
}
