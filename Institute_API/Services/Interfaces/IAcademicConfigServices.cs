using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Repository.Interfaces
{
    public interface IAcademicConfigServices
    {
        Task<ServiceResponse<string>> AddUpdateAcademicConfig(Class request);
        Task<ServiceResponse<string>> DeleteAcademicConfig(int ClassId);
        Task<ServiceResponse<Class>> GetAcademicConfigById(int ClassId);
        Task<ServiceResponse<List<Class>>> GetAcademicConfigList(GetAllCourseClassRequest request);
        Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId, string format);
    }
}
