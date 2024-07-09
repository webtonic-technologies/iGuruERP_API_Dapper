using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;

namespace Institute_API.Repository.Interfaces
{
    public interface IAcademicConfigRepository
    {
        Task<ServiceResponse<string>> AddUpdateAcademicConfig(Class request);
        Task<ServiceResponse<string>> DeleteAcademicConfig(int ClassId);
        Task<ServiceResponse<Class>> GetAcademicConfigById(int ClassId);
        Task<ServiceResponse<List<Class>>> GetAcademicConfigList(GetAllCourseClassRequest request);
    }
}
