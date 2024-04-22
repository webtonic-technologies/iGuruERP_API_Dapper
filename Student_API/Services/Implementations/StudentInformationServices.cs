using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Services.Implementations
{
    public class StudentInformationServices : IStudentInformationServices
    {
        private readonly IStudentInformationRepository _studentInformationRepository;
        public StudentInformationServices(IStudentInformationRepository studentInformationRepository)
        {
            _studentInformationRepository = studentInformationRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasterDTO request)
        {
            var data = await _studentInformationRepository.AddUpdateStudentInformation(request);
            return data;
        }

        public async Task<ServiceResponse<StudentMasterDTO>> GetStudentDetailsById(int studentId)
        {
            var data = await _studentInformationRepository.GetStudentDetailsById(studentId);
            return data;
        }
        public async Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails()
        {

            try
            {
                return await _studentInformationRepository.GetAllStudentDetails();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentDetailsDTO>>(false, ex.Message, [], 500);
            }
        }
        public async Task<ServiceResponse<int>> ChangeStudentStatus(StudentStatusDTO statusDTO)
        {
            var data = await _studentInformationRepository.ChangeStudentStatus(statusDTO);
            return data;
        }
    }
}
