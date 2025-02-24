using System.Threading.Tasks;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Response.StudentManagement;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;

namespace StudentManagement_API.Repository.Interfaces
{
    public interface IStudentInformationRepository
    {
        Task<ServiceResponse<string>> AddUpdateStudent(AddUpdateStudentRequest request);
        Task<ServiceResponse<IEnumerable<GetStudentInformationResponse>>> GetStudentInformation(GetStudentInformationRequest request);
        Task<ServiceResponse<string>> SetStudentStatusActivity(SetStudentStatusActivityRequest request);
        Task<ServiceResponse<IEnumerable<GetStudentStatusActivityResponse>>> GetStudentStatusActivity(GetStudentStatusActivityRequest request);
        Task<DownloadStudentImportTemplateResponse> GetMasterTablesData(int instituteID);
        //Task<ServiceResponse<string>> InsertStudents(int instituteID, List<StudentInformationImportRequest> students);
        Task<ServiceResponse<string>> InsertStudents(int instituteID, string AcademicYearCode, string IPAddress, int UserID, List<StudentInformationImportRequest> students);
        Task<List<SectionJoinedResponse>> GetSectionsWithClassNames(int instituteID);
        Task<ServiceResponse<IEnumerable<GetStudentSettingResponse>>> GetStudentSetting(GetStudentSettingRequest request);
        Task<ServiceResponse<string>> AddRemoveStudentSetting(List<AddRemoveStudentSettingRequest> request);

        //Task<List<GetStudentInformationResponse>> GetStudentInformationExport(GetStudentInformationExportRequest request);
        Task<List<GetStudentInformationResponse>> GetStudentInformationExport(int instituteID, string AcademicYearCode, string IPAddress, int UserID, GetStudentInformationExportRequest request);
        Task<IEnumerable<GetStudentImportHistoryResponse>> GetStudentImportHistoryAsync(GetStudentImportHistoryRequest request);
        Task<IEnumerable<GetStudentExportHistoryResponse>> GetStudentExportHistoryAsync(GetStudentExportHistoryRequest request);

    }
}



 