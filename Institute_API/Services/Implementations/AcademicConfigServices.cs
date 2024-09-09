using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class AcademicConfigServices : IAcademicConfigServices
    {
        private readonly IAcademicConfigRepository _academicConfigRepository;

        public AcademicConfigServices(IAcademicConfigRepository academicConfigRepository)
        {
            _academicConfigRepository = academicConfigRepository;
        }
        public async Task<ServiceResponse<string>> AddUpdateAcademicConfig(Class request)
        {
            try
            {
                return await _academicConfigRepository.AddUpdateAcademicConfig(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteAcademicConfig(int ClassId)
        {
            try
            {
                return await _academicConfigRepository.DeleteAcademicConfig(ClassId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId)
        {
            try
            {
                return await _academicConfigRepository.DownloadExcelSheet(InstituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<Class>> GetAcademicConfigById(int ClassId)
        {
            try
            {
                return await _academicConfigRepository.GetAcademicConfigById(ClassId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Class>(false, ex.Message, new Class(), 500);
            }
        }

        public async Task<ServiceResponse<List<Class>>> GetAcademicConfigList(GetAllCourseClassRequest request)
        {
            try
            {
                return await _academicConfigRepository.GetAcademicConfigList(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Class>>(false, ex.Message, [], 500);
            }
        }
    }
}
