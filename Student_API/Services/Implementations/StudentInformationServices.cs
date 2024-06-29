using QRCoder;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace Student_API.Services.Implementations
{
    public class StudentInformationServices : IStudentInformationServices
    {
        private readonly IStudentInformationRepository _studentInformationRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IImageService _imageService;
        public StudentInformationServices(IImageService imageService, IStudentInformationRepository studentInformationRepository, IWebHostEnvironment webHostEnvironment)
        {
            _studentInformationRepository = studentInformationRepository;
            _hostingEnvironment = webHostEnvironment;
            _imageService = imageService;
        }


        public async Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasters request)
        {
            try
            {

                if (request.File_Name != null && request.File_Name != "")
                {
                    var file = await _imageService.SaveImageAsync(request.File_Name, "StudentsInfoFile");
                    if (request.student_id != 0)
                    {
                        var ImageName = await _studentInformationRepository.GetStudentInfoImageById(request.student_id);
                        if (ImageName.Data != null & ImageName.Data != "")
                        {
                            _imageService.DeleteFile(ImageName.Data);
                        }
                    }
                    request.File_Name = file.relativePath;
                }
                if (request.student_id == 0)
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(request.First_Name + "  " + request.Last_Name, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCodeImage = new QRCode(qrCodeData);

                    using (Bitmap bitmap = qrCodeImage.GetGraphic(60))
                    {
                        string base64String;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitmap.Save(ms, ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            base64String = Convert.ToBase64String(byteImage);
                        }

                        var result = await _imageService.SaveImageAsync(base64String, "QrCodes");
                        request.QR_code = result.relativePath;
                    }
                }

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
                if (data.Data != null)
                {
                    if (data.Data != null && data.Data.File_Name != null && data.Data.File_Name != "")
                    {
                        data.Data.File_Name = _imageService.GetImageAsBase64(data.Data.File_Name);
                    }

                    if (data.Data.studentParentInfos != null)
                    {
                        foreach (var studentParentInfos in data.Data.studentParentInfos)
                        {
                            if (!string.IsNullOrEmpty(studentParentInfos.File_Name) && File.Exists(studentParentInfos.File_Name))
                            {
                                studentParentInfos.File_Name = _imageService.GetImageAsBase64(studentParentInfos.File_Name);
                            }
                        }
                    }
                    if (data.Data != null && data.Data.studentDocumentListDTOs != null)
                    {
                        foreach (var document in data.Data.studentDocumentListDTOs)
                        {
                            if (!string.IsNullOrEmpty(document.File_Name) && File.Exists(document.File_Name))
                            {
                                document.Document = _imageService.GetImageAsBase64(document.File_Name);
                            }
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentInformationDTO>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails(int Institute_id, string sortField = "Student_Name", string sortDirection = "ASC", int? pageNumber = null, int? pageSize = null)
        {

            try
            {
                return await _studentInformationRepository.GetAllStudentDetails(Institute_id, sortField, sortDirection, pageNumber, pageSize);
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
        public async Task<ServiceResponse<int>> AddUpdateStudentOtherInfo(StudentOtherInfos request)
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
        public async Task<ServiceResponse<int>> AddUpdateStudentParentInfo(StudentParentInfo request)
        {
            try
            {
                if (request.File_Name != null && request.File_Name != "")
                {
                    var file = await _imageService.SaveImageAsync(request.File_Name, "StudentsInfoFile");
                    if (request.Student_Parent_Info_id != 0)
                    {
                        var ImageName = await _studentInformationRepository.GetStudentparentImageById(request.Student_Parent_Info_id);
                        if (ImageName.Data != null & ImageName.Data != "")
                        {
                            _imageService.DeleteFile(ImageName.Data);
                        }
                    }
                    request.File_Name = file.relativePath;
                }
                var data = await _studentInformationRepository.AddUpdateStudentParentInfo(request);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddOrUpdateStudentSiblings(StudentSibling sibling)
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
        public async Task<ServiceResponse<int>> AddOrUpdateStudentPreviousSchool(StudentPreviousSchools previousSchool)
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
        public async Task<ServiceResponse<int>> AddOrUpdateStudentHealthInfo(StudentHealthInfos healthInfo)
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
                foreach (var item in request.File_Name)
                {
                    StudentDocumentListDTO listDTO = new StudentDocumentListDTO();

                    var data = await _imageService.SaveImageAsync(item, "StudentsDoc");
                    //var fileName = data.fileName;
                    //var relativePath = data.relativePath;
                    //var absolutePath = data.absolutePath;
                    //var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "StudentsDoc");
                    //if (!Directory.Exists(uploads))
                    //{
                    //    Directory.CreateDirectory(uploads);
                    //}
                    //var fileName = Path.GetFileNameWithoutExtension(item.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                    //var filePath = Path.Combine(uploads, fileName);
                    //using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    //{
                    //    await item.CopyToAsync(fileStream);
                    //}

                    //if (item.Student_Documents_id > 0)
                    //{
                    //    var oldFilePath = Path.Combine(uploads, item.File_Name);
                    //    if (File.Exists(oldFilePath))
                    //    {
                    //        File.Delete(oldFilePath);
                    //    }

                    //}
                    listDTO.File_Name = data.relativePath;
                    listDTO.File_Path = data.absolutePath;
                    listDTO.Document_Name = data.fileName;

                    await _studentInformationRepository.AddUpdateStudentDocuments(listDTO, request.Student_id);
                }
                return new ServiceResponse<int>(true, "Operation successful", 1, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }

        }

        public async Task<ServiceResponse<int>> DeleteStudentDocument(int Student_Documents_id)
        {
            try
            {
                var data = await _studentInformationRepository.DeleteStudentDocument(Student_Documents_id);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
    }
}
