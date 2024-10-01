using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Models;
using TimeTable_API.Repository.Interfaces;
using TimeTable_API.Services.Interfaces;
using TimeTable_API.DTOs.Responses;

namespace TimeTable_API.Services.Implementations
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
            try
            {
                return await _groupRepository.AddUpdateGroup(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ServiceResponse<List<GroupResponse>>> GetAllGroups(GetAllGroupsRequest request)
        {
            // Call the repository method to get GroupResponse
            var response = await _groupRepository.GetAllGroups(request);

            if (response.Success)
            {
                // Return the GroupResponse directly as it contains all necessary fields
                return new ServiceResponse<List<GroupResponse>>(true, response.Message, response.Data, response.StatusCode);
            }

            // If the response was not successful, return the original error message
            return new ServiceResponse<List<GroupResponse>>(response.Success, response.Message, new List<GroupResponse>(), response.StatusCode);
        }



        public async Task<ServiceResponse<GroupResponse>> GetGroupById(int groupId)
        {
            // Delegate to repository and return the response
            return await _groupRepository.GetGroupById(groupId);
        }

        public async Task<ServiceResponse<bool>> DeleteGroup(int groupId)
        {
            try
            {
                return await _groupRepository.DeleteGroup(groupId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
    }
}
