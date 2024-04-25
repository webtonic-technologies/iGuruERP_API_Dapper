using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Repository.Interfaces
{
    public interface IAcademicConfigServices
    {
        Task<ServiceResponse<string>> AddUpdateAcademicConfig(CourseClassDTO request);
        Task<ServiceResponse<string>> DeleteAcademicConfig(int CourseClass_id);
        Task<ServiceResponse<CourseClassDTO>> GetAcademicConfigById(int CourseClass_id);
        Task<ServiceResponse<List<CourseClassDTO>>> GetAcademicConfigList(int Institute_id);
    }
}
