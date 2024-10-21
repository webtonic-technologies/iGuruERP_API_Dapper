using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IStudentPromotionService
    {
        Task<ServiceResponse<List<StudentPromotionDTO>>> GetStudentsForPromotion(GetStudentsForPromotionParam obj);
        Task<ServiceResponse<bool>> PromoteStudents(List<int> studentIds, int nextClassId, int sectionId, int CurrentAcademicYear);
        Task<ServiceResponse<bool>> PromoteClasses(ClassPromotionDTO classPromotionDTO);
        Task<ServiceResponse<List<ClassPromotionLogDTO>>> GetClassPromotionLog(GetClassPromotionLogParam obj);
        Task<ServiceResponse<string>> ExportClassPromotionLogToExcel(ExportClassPromotionLogParam obj);
        Task<ServiceResponse<int?>> GetToClassIdAsync(ClassPromotionParams classPromotionDTO);
    }
}
