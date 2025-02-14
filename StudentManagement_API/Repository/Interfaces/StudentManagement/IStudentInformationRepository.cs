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
        Task<ServiceResponse<string>> InsertStudents(List<StudentInformationImportRequest> students);

    }
}
