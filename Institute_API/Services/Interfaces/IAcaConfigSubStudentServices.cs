using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using static Institute_API.Repository.Implementations.AcaConfigSubStudentRepository;
namespace Institute_API.Services.Interfaces
{
    public interface IAcaConfigSubStudentServices
    {
        Task<ServiceResponse<string>> AddUpdateSubjectStudentMapping(AcaConfigSubStudentRequest request);
       // Task<ServiceResponse<AcaConfigSubStudentResponse>> GetSubjectStudentMappingList(MappingListRequest request);
        Task<ServiceResponse<List<StudentListResponse>>> GetInstituteStudentsList(StudentListRequest request);
       // Task<ServiceResponse<List<SubjectList>>> GetInstituteSubjectsList(int SubjectType);
        Task<ServiceResponse<byte[]>> DownloadExcelSheet(ExcelDownloadSubStudentMappingRequest request);
        Task<ServiceResponse<CombinedResponse>> GetSubjectsAndStudentMappings(MappingListRequest request);
    }
}
