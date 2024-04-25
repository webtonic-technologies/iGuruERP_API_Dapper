using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;

namespace Student_API.Services.Implementations
{
    public class StudentInformationServices : IStudentInformationServices
    {
        private readonly IStudentInformationRepository _studentInformationRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public StudentInformationServices(IStudentInformationRepository studentInformationRepository , IWebHostEnvironment webHostEnvironment)
        {
            _studentInformationRepository = studentInformationRepository;
            _hostingEnvironment = webHostEnvironment;
        }

        public async Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasterDTO request)
        {
            try
            {
                var data = await _studentInformationRepository.AddUpdateStudentInformation(request);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<StudentInformationDTO>> GetStudentDetailsById(int studentId)
        {
            try
            {
                var data = await _studentInformationRepository.GetStudentDetailsById(studentId);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentInformationDTO>(false, ex.Message, null, 500);
            }
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
            try
            {
                var data = await _studentInformationRepository.ChangeStudentStatus(statusDTO);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateStudentOtherInfo(StudentOtherInfoDTO request)
        {
            try
            {
                var data = await _studentInformationRepository.AddUpdateStudentOtherInfo(request);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateStudentParentInfo(StudentParentInfoDTO request)
        {
            try
            {
                var data = await _studentInformationRepository.AddUpdateStudentParentInfo(request);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddOrUpdateStudentSiblings(StudentSiblings sibling)
        {
            try
            {
                var data = await _studentInformationRepository.AddOrUpdateStudentSiblings(sibling);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddOrUpdateStudentPreviousSchool(StudentPreviousSchool previousSchool)
        {
            try
            {
                var data = await _studentInformationRepository.AddOrUpdateStudentPreviousSchool(previousSchool);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddOrUpdateStudentHealthInfo(StudentHealthInfo healthInfo)
        {
            try
            {
                var data = await _studentInformationRepository.AddOrUpdateStudentHealthInfo(healthInfo);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateStudentDocuments(StudentDocumentsDTO request)
        {
            try
            {
                foreach (var item in request.formFiles)
                {
					StudentDocumentListDTO listDTO = new StudentDocumentListDTO();  
					var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "StudentsDoc");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var fileName = Path.GetFileNameWithoutExtension(item.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(uploads, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await item.CopyToAsync(fileStream);
                    }

					//if (item.Student_Documents_id > 0)
					//{
					//    var oldFilePath = Path.Combine(uploads, item.File_Name);
					//    if (File.Exists(oldFilePath))
					//    {
					//        File.Delete(oldFilePath);
					//    }

					//}
					listDTO.File_Name = fileName;
					listDTO.File_Path = filePath;
					listDTO.Document_Name = item.FileName;
                   
                    await _studentInformationRepository.AddUpdateStudentDocuments(listDTO, request.Student_id);
                }
                return new ServiceResponse<int>(true, "Operation successful", 1, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
    }
}
