using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IStudentPromotionService
    {
        Task<ServiceResponse<List<StudentPromotionDTO>>> GetStudentsForPromotion(int classId, string sortField, string sortDirection, int? pageSize = null, int? pageNumber = null);
        Task<ServiceResponse<bool>> PromoteStudents(List<int> studentIds, int nextClassId);
    }
}
