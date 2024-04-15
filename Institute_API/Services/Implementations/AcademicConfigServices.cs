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
        public async Task<ServiceResponse<string>> AddUpdateAcademicConfig(CourseClassDTO request)
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

        public async Task<ServiceResponse<string>> DeleteAcademicConfig(int CourseClass_id)
        {
            try
            {
                return await _academicConfigRepository.DeleteAcademicConfig(CourseClass_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<CourseClassDTO>> GetAcademicConfigById(int CourseClass_id)
        {
            try
            {
                return await _academicConfigRepository.GetAcademicConfigById(CourseClass_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CourseClassDTO>(false, ex.Message, new CourseClassDTO(), 500);
            }
        }

        public async Task<ServiceResponse<List<CourseClassDTO>>> GetAcademicConfigList(int Institute_id)
        {
            try
            {
                return await _academicConfigRepository.GetAcademicConfigList(Institute_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CourseClassDTO>>(false, ex.Message, [], 500);
            }
        }
    }
}
