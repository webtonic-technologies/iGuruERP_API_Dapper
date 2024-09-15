using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class AcademicConfigSubjectsServices : IAcademicConfigSubjectsServices
    {
        private readonly IAcademicConfigSubjectsRepository _academicConfigSubjectsRepository;

        public AcademicConfigSubjectsServices(IAcademicConfigSubjectsRepository academicConfigSubjectsRepository)
        {
            _academicConfigSubjectsRepository = academicConfigSubjectsRepository;
        }
        public async Task<ServiceResponse<string>> AddUpdateAcademicConfigSubject(SubjectRequest request)
        {
            try
            {
                return await _academicConfigSubjectsRepository.AddUpdateAcademicConfigSubject(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> AddUpdateSubjectType(SubjectType request)
        {
            try
            {
                return await _academicConfigSubjectsRepository.AddUpdateSubjectType(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteAcademicConfigSubject(int SubjectId)
        {
            try
            {
                return await _academicConfigSubjectsRepository.DeleteAcademicConfigSubject(SubjectId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(ExcelDownloadRequest request)
        {
            try
            {
                return await _academicConfigSubjectsRepository.DownloadExcelSheet(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<SubjectResponse>> GetAcademicConfigSubjectById(int SubjectId)
        {
            try
            {
                return await _academicConfigSubjectsRepository.GetAcademicConfigSubjectById(SubjectId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SubjectResponse>(false, ex.Message, new SubjectResponse(), 500);
            }
        }

        public async Task<ServiceResponse<List<SubjectResponse>>> GetAcademicConfigSubjectList(GetAllSubjectRequest request)
        {
            try
            {
                return await _academicConfigSubjectsRepository.GetAcademicConfigSubjectList(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubjectResponse>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<SubjectType>> GetSubjectTypeById(int subjectTypeId)
        {
            try
            {
                return await _academicConfigSubjectsRepository.GetSubjectTypeById(subjectTypeId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SubjectType>(false, ex.Message, new SubjectType(), 500);
            }
        }

        public async Task<ServiceResponse<List<SubjectType>>> GetSubjectTypeList()
        {
            try
            {
                return await _academicConfigSubjectsRepository.GetSubjectTypeList();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubjectType>>(false, ex.Message, [], 500);
            }
        }
    }
}