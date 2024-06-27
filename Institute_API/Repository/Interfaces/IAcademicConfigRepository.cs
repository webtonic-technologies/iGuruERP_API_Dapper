using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;

namespace Institute_API.Repository.Interfaces
{
    public interface IAcademicConfigRepository
    {
        Task<ServiceResponse<string>> AddUpdateAcademicConfig(CourseClassDTO request);
        Task<ServiceResponse<string>> DeleteAcademicConfig(int CourseClass_id);
        Task<ServiceResponse<CourseClassDTO>> GetAcademicConfigById(int CourseClass_id);
        Task<ServiceResponse<List<CourseClassDTO>>> GetAcademicConfigList(GetAllCourseClassRequest request);
    }
}
