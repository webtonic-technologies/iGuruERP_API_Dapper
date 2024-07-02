using Communication_API.DTOs.Requests.Configuration;
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

        public async Task<ServiceResponse<List<Group>>> GetAllGroup(GetAllGroupRequest request)
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
    }
}
