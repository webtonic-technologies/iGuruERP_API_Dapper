using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IStudentQRService
    {
        Task<ServiceResponse<List<StudentQRDTO>>> GetAllStudentQR(int sectionId, int classId, string sortField, string sortDirection, string searchQuery = null, int? pageNumber = null, int? pageSize = null);
    }
}
