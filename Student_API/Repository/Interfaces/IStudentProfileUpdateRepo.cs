
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;


namespace Student_API.Repository.Interfaces
{
    public interface IStudentProfileUpdateRepo
    {
        Task<ServiceResponse<string>> AddProfileUpdateRequest(int studentId, int status);
        Task<ServiceResponse<string>> UpdateProfileUpdateRequest(int requestId, int newStatus);
        Task<ServiceResponse<List<ProfileUpdateRequestDTO>>> GetProfileUpdateRequests(GetStudentProfileRequestModel obj);
    }
}
