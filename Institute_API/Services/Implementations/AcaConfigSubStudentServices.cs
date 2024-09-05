using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class AcaConfigSubStudentServices : IAcaConfigSubStudentServices
    {
        private readonly IAcaConfigSubStudentRepository _acaConfigSubStudentRepository;

        public AcaConfigSubStudentServices(IAcaConfigSubStudentRepository acaConfigSubStudentRepository)
        {
            _acaConfigSubStudentRepository = acaConfigSubStudentRepository;
        }
        public async Task<ServiceResponse<string>> AddUpdateSubjectStudentMapping(AcaConfigSubStudentRequest request)
        {
            try
            {
                return await _acaConfigSubStudentRepository.AddUpdateSubjectStudentMapping(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<List<StudentListResponse>>> GetInstituteStudentsList(StudentListRequest request)
        {
            try
            {
                return await _acaConfigSubStudentRepository.GetInstituteStudentsList(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentListResponse>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<List<SubjectList>>> GetInstituteSubjectsList(int SubjectType)
        {
            try
            {
                return await _acaConfigSubStudentRepository.GetInstituteSubjectsList(SubjectType);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubjectList>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<AcaConfigSubStudentResponse>> GetSubjectStudentMappingList(MappingListRequest request)
        {
            try
            {
                return await _acaConfigSubStudentRepository.GetSubjectStudentMappingList(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AcaConfigSubStudentResponse>(false, ex.Message, new AcaConfigSubStudentResponse(), 500);
            }
        }
    }
}
