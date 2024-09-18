
using Student_API.DTOs.ServiceResponse;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;

namespace Student_API.Services.Interfaces
{
    public interface IStudentProfileUpdateServices
    {
        Task<ServiceResponse<string>> AddProfileUpdateRequest(int studentId, int status);
        Task<ServiceResponse<string>> UpdateProfileUpdateRequest(int requestId, int newStatus);
        Task<ServiceResponse<List<ProfileUpdateRequestDTO>>> GetProfileUpdateRequests(GetStudentProfileRequestModel obj);
    }
}
