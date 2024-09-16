using Communication_API.DTOs.Requests;
using Communication_API.DTOs.Requests.Configuration;
using Communication_API.DTOs.Responses.Configuration;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Configuration;
using Communication_API.Repository.Interfaces.Configuration;
using Communication_API.Services.Interfaces.Configuration;

namespace Communication_API.Services.Implementations.Configuration
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateGroup(AddUpdateGroupRequest request)
        {
            return await _groupRepository.AddUpdateGroup(request);
        }

        public async Task<ServiceResponse<List<GetAllGroupResponse>>> GetAllGroup(GetAllGroupRequest request)
        {
            return await _groupRepository.GetAllGroup(request);
        }

        public async Task<ServiceResponse<Group>> GetGroup(int GroupID)
        {
            return await _groupRepository.GetGroup(GroupID);
        }

        public async Task<ServiceResponse<string>> DeleteGroup(int GroupID)
        {
            return await _groupRepository.DeleteGroup(GroupID);
        }

        public async Task<ServiceResponse<List<GetGroupUserTypeResponse>>> GetGroupUserTypes()
        {
            return await _groupRepository.GetGroupUserTypes();
        }

        public async Task<ServiceResponse<GetGroupMembersResponse>> GetGroupMembers(GetGroupMembersRequest request)
        {
            return await _groupRepository.GetGroupMembers(request);
        }



    }
}
