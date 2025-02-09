using Communication_API.DTOs.Requests;
using Communication_API.DTOs.Responses;
using Communication_API.DTOs.ServiceResponse;

namespace Communication_API.Repository.Interfaces
{
    public interface ICommunicationUserRepository
    {
        Task<ServiceResponse<List<GetStudentListResponse>>> GetStudentList(GetStudentListRequest request);
        Task<ServiceResponse<List<GetEmployeeListResponse>>> GetEmployeeList(GetEmployeeListRequest request);

    }
}
