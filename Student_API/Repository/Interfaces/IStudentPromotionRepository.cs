using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Repository.Interfaces
{
    public interface IStudentPromotionRepository
    {
        Task<ServiceResponse<List<StudentPromotionDTO>>> GetStudentsForPromotion(int classId, string sortField, string sortDirection, int? pageSize = null, int? pageNumber = null);
        Task<ServiceResponse<bool>> PromoteStudents(List<int> studentIds, int nextClassId, int sectionId, int CurrentAcademicYear);
        Task<ServiceResponse<bool>> PromoteClasses(ClassPromotionDTO classPromotionDTO);
        Task<ServiceResponse<List<ClassPromotionLogDTO>>> GetClassPromotionLog(GetClassPromotionLogParam obj);
        Task<ServiceResponse<int?>> GetToClassIdAsync(int fromClassId, int instituteId);
    }
}
