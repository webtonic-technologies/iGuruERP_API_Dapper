using Communication_API.DTOs.Requests.Configuration;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Configuration;

namespace Communication_API.Services.Interfaces.Configuration
{
    public interface IGroupService
    {
        Task<ServiceResponse<string>> AddUpdateGroup(AddUpdateGroupRequest request);
        Task<ServiceResponse<List<Group>>> GetAllGroup(GetAllGroupRequest request);
        Task<ServiceResponse<Group>> GetGroup(int GroupID);
        Task<ServiceResponse<string>> DeleteGroup(int GroupID);
    }
}
