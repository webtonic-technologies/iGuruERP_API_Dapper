using Communication_API.DTOs.Requests;
using Communication_API.DTOs.Responses;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Repository.Interfaces;
using Communication_API.Services.Interfaces;

namespace Communication_API.Services.Implementations
{
    public class CommunicationUserService : ICommunicationUserService
    {
        private readonly ICommunicationUserRepository _communicationUserRepository;

        public CommunicationUserService(ICommunicationUserRepository communicationUserRepository)
        {
            _communicationUserRepository = communicationUserRepository;
        }

        public async Task<ServiceResponse<List<GetStudentListResponse>>> GetStudentList(GetStudentListRequest request)
        {
            return await _communicationUserRepository.GetStudentList(request);
        }

        public async Task<ServiceResponse<List<GetEmployeeListResponse>>> GetEmployeeList(GetEmployeeListRequest request)
        {
            return await _communicationUserRepository.GetEmployeeList(request);
        }
    }
}
