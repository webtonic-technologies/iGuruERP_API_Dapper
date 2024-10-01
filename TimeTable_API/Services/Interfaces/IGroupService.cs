using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.Models;

namespace TimeTable_API.Services.Interfaces
{
    public interface IGroupService
    {
        Task<ServiceResponse<string>> AddUpdateGroup(AddUpdateGroupRequest request);
        Task<ServiceResponse<List<GroupResponse>>> GetAllGroups(GetAllGroupsRequest request);
        Task<ServiceResponse<GroupResponse>> GetGroupById(int groupId);
        Task<ServiceResponse<bool>> DeleteGroup(int groupId);
    }
}
