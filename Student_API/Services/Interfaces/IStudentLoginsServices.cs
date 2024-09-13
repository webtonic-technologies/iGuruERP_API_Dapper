using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
namespace Student_API.Services.Interfaces
{
    public interface IStudentLoginsServices
    {
        Task<ServiceResponse<List<StudentCredentialsResponse>>> GetAllStudentLoginCred(StudentLoginRequest request);
        Task<ServiceResponse<List<StudentsNonAppUsersResponse>>> GetAllStudentNonAppUsers(StudentLoginRequest request);
        Task<ServiceResponse<List<StudentActivityResponse>>> GetAllStudentActivity(StudentLoginRequest request);
        Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId);
        Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(int InstituteId);
        Task<ServiceResponse<byte[]>> DownloadExcelSheetStudentActivity(int InstituteId);
    }
}
