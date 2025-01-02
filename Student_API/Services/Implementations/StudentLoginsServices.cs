using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Services.Implementations
{
    public class StudentLoginsServices : IStudentLoginsServices
    {

        private readonly IStudentLoginsRepository _studentLoginsRepository;

        public StudentLoginsServices(IStudentLoginsRepository studentLoginsRepository)
        {
            _studentLoginsRepository = studentLoginsRepository;
        }
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId, string format)
        {
            return await _studentLoginsRepository.DownloadExcelSheet(InstituteId, format);
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetNonAppUsers(int InstituteId, string format)
        {
            return await _studentLoginsRepository.DownloadExcelSheetNonAppUsers(InstituteId, format);
        }

        public async Task<ServiceResponse<byte[]>> DownloadExcelSheetStudentActivity(int InstituteId, string fileType)
        {
            return await _studentLoginsRepository.DownloadExcelSheetStudentActivity(InstituteId, fileType);
        }

        public async Task<ServiceResponse<List<StudentActivityResponse>>> GetAllStudentActivity(StudentLoginRequest request)
        {
            return await _studentLoginsRepository.GetAllStudentActivity(request);
        }

        public async Task<ServiceResponse<List<StudentCredentialsResponse>>> GetAllStudentLoginCred(StudentLoginRequest request)
        {
            return await _studentLoginsRepository.GetAllStudentLoginCred(request);
        }

        public async Task<ServiceResponse<List<StudentsNonAppUsersResponse>>> GetAllStudentNonAppUsers(StudentLoginRequest request)
        {
            return await _studentLoginsRepository.GetAllStudentNonAppUsers(request);
        }
    }
}
