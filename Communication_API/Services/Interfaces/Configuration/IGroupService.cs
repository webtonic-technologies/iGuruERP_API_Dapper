using Communication_API.DTOs.Requests;
using Communication_API.DTOs.Requests.Configuration;
using Communication_API.DTOs.Responses.Configuration;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Configuration;

namespace Communication_API.Services.Interfaces.Configuration
{
    public interface IGroupService
    {
        Task<ServiceResponse<string>> AddUpdateGroup(AddUpdateGroupRequest request);
        Task<ServiceResponse<List<GetAllGroupResponse>>> GetAllGroup(GetAllGroupRequest request);
        Task<ServiceResponse<Group>> GetGroup(int GroupID);
        Task<ServiceResponse<string>> DeleteGroup(int GroupID);
        Task<ServiceResponse<List<GetGroupUserTypeResponse>>> GetGroupUserTypes();
        Task<ServiceResponse<GetGroupMembersResponse>> GetGroupMembers(GetGroupMembersRequest request);

    }
}
