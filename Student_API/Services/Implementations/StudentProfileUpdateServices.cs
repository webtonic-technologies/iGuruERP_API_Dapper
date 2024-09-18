using Student_API.Services.Interfaces;
using Student_API.Repository.Interfaces;
using Student_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;

namespace Student_API.Services.Implementations
{
    public class StudentProfileUpdateServices : IStudentProfileUpdateServices
    {
        private readonly IStudentProfileUpdateRepo _repo;

        public StudentProfileUpdateServices(IStudentProfileUpdateRepo repo)
        {
            _repo = repo;
        }

        public async Task<ServiceResponse<string>> AddProfileUpdateRequest(int studentId, int status)
        {
            return await _repo.AddProfileUpdateRequest(studentId, status);
        }

        public async Task<ServiceResponse<string>> UpdateProfileUpdateRequest(int requestId, int newStatus)
        {
            return await _repo.UpdateProfileUpdateRequest(requestId, newStatus);
        }

        public async Task<ServiceResponse<List<ProfileUpdateRequestDTO>>> GetProfileUpdateRequests(GetStudentProfileRequestModel obj)
        {
            return await _repo.GetProfileUpdateRequests(obj);
        }
    }
}
