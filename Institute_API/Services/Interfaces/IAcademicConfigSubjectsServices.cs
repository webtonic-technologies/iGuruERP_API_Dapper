using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Services.Interfaces
{
    public interface IAcademicConfigSubjectsServices
    {
        Task<ServiceResponse<string>> AddUpdateAcademicConfigSubject(SubjectRequest request);
        Task<ServiceResponse<string>> DeleteAcademicConfigSubject(int SubjectId);
        Task<ServiceResponse<SubjectResponse>> GetAcademicConfigSubjectById(int SubjectId);
        Task<ServiceResponse<List<SubjectResponse>>> GetAcademicConfigSubjectList(GetAllSubjectRequest request);
        Task<ServiceResponse<string>> AddUpdateSubjectType(SubjectType request);
        Task<ServiceResponse<List<SubjectType>>> GetSubjectTypeList();
        Task<ServiceResponse<SubjectType>> GetSubjectTypeById(int subjectTypeId);
        Task<ServiceResponse<byte[]>> DownloadSubjectData(ExcelDownloadRequest request, string format);
    }
}
