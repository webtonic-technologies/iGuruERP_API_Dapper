using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
namespace Institute_API.Services.Interfaces
{
    public interface IAcaConfigSubStudentServices
    {
        Task<ServiceResponse<string>> AddUpdateSubjectStudentMapping(AcaConfigSubStudentRequest request);
        Task<ServiceResponse<AcaConfigSubStudentRequest>> GetSubjectStudentMappingList(MappingListRequest request);
        Task<ServiceResponse<List<StudentListResponse>>> GetInstituteStudentsList(StudentListRequest request);
        Task<ServiceResponse<List<SubjectList>>> GetInstituteSubjectsList(int SubjectType);
    }
}
