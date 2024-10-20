﻿using OfficeOpenXml;
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

        public async Task<ServiceResponse<int>> AddUpdateStudent(StudentDTO request)
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

                        var result = await _imageService.SaveImageAsync(base64String, "Insititute_" + request.Institute_id + "/QrCodes");
                        request.QR_code = result.relativePath;
                    }
                }
                foreach (var item in request.studentParentInfos)
                {
                    if (item.File_Name != null && item.File_Name != "")
                    {
                        var file = await _imageService.SaveImageAsync(item.File_Name, "StudentsInfoFile");
                        if (item.Student_Parent_Info_id != 0)
                        {
                            var ImageName = await _studentInformationRepository.GetStudentparentImageById(item.Student_Parent_Info_id);
                            if (ImageName.Data != null & ImageName.Data != "")
                            {
                                _imageService.DeleteFile(ImageName.Data);
                            }
                        }
                        item.File_Name = file.relativePath;
                    }
                }
                List<StudentDocumentListDTO> studentDocuments = new();
                foreach (var item in request.studentDocuments.File_Name)
                {
                    if (item != null && item != "")
                    {
                        StudentDocumentListDTO listDTO = new StudentDocumentListDTO();

                        var doc = await _imageService.SaveImageAsync(item, "StudentsDoc");

                        listDTO.File_Name = doc.relativePath;
                        listDTO.File_Path = doc.absolutePath;
                        listDTO.Document_Name = doc.fileName;

                        studentDocuments.Add(listDTO);
                    }
                }


                var data = await _studentInformationRepository.AddUpdateStudent(request, studentDocuments);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
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
        public async Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails(GetStudentRequestModel obj)
        {

            try
            {
                return await _studentInformationRepository.GetAllStudentDetails(obj);
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

        public async Task<ServiceResponse<List<StudentInformationDTO>>> GetAllStudentDetailsData(GetStudentRequestModel obj)
        {

            try
            {
                return await _studentInformationRepository.GetAllStudentDetailsData(obj);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentInformationDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<StudentAllInformationDTO>>> GetAllStudentDetailsData1(GetStudentRequestModel obj)
        {

            try
            {
                return await _studentInformationRepository.GetAllStudentDetailsData1(obj);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentAllInformationDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> GetAllStudentDetailsAsExcel(getStudentRequest obj)
        {
            try
            {
                GetStudentRequestModel model = new GetStudentRequestModel();    
                model.Academic_year_id = obj.Academic_year_id;
                model.section_id = obj.section_id;  
                model.class_id = obj.class_id;
                model.Institute_id = obj.Institute_id;
                model.isActive = obj.isActive;
                model.pageSize = int.MaxValue;
                model.pageNumber = 1;
                model.sortField = null;
                model.sortDirection = null; 
                // Call the existing method to get the data
                var studentDetailsResponse = await _studentInformationRepository.GetAllStudentDetails(model);

                if ( studentDetailsResponse.Data.Any())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    // Create an Excel package using EPPlus
                    using (var package = new ExcelPackage())
                    {
                        // Create a worksheet in the Excel workbook
                        var worksheet = package.Workbook.Worksheets.Add("StudentDetails");

                        // Add headers
                        worksheet.Cells[1, 1].Value = "Student Name";
                        worksheet.Cells[1, 2].Value = "Class";
                        worksheet.Cells[1, 3].Value = "Section";
                        worksheet.Cells[1, 4].Value = "Admission Number";
                        worksheet.Cells[1, 5].Value = "Roll Number";
                        worksheet.Cells[1, 6].Value = "Date of Joining";
                        worksheet.Cells[1, 7].Value = "Date of Birth";
                        worksheet.Cells[1, 8].Value = "Religion";
                        worksheet.Cells[1, 9].Value = "Gender";
                        worksheet.Cells[1, 10].Value = "Father's Name";

                        // Add data rows
                        var rowIndex = 2; // Start from row 2 as row 1 contains headers
                        foreach (var student in studentDetailsResponse.Data)
                        {
                            worksheet.Cells[rowIndex, 1].Value = student.Student_Name;
                            worksheet.Cells[rowIndex, 2].Value = student.class_course;
                            worksheet.Cells[rowIndex, 3].Value = student.Section;
                            worksheet.Cells[rowIndex, 4].Value = student.Admission_Number;
                            worksheet.Cells[rowIndex, 5].Value = student.Roll_Number;
                            worksheet.Cells[rowIndex, 6].Value = student.Date_of_Joining;
                            worksheet.Cells[rowIndex, 7].Value = student.Date_of_Birth;
                            worksheet.Cells[rowIndex, 8].Value = student.Religion_Type;
                            worksheet.Cells[rowIndex, 9].Value = student.Gender_Type;
                            worksheet.Cells[rowIndex, 10].Value = student.Father_Name;
                            rowIndex++;
                        }

                        // Auto-fit columns for better readability
                        worksheet.Cells.AutoFitColumns();

                        // Generate Excel file as a byte array
                        var excelFile = package.GetAsByteArray();

                        // Save the file to a specific location or return the file content as a downloadable response
                        var fileName = $"StudentDetails_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);

                        // Ensure the directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        // Write file to the disk
                        await File.WriteAllBytesAsync(filePath, excelFile);

                        // Return the file path as a response
                        return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
                    }
                }
                else
                {
                    return new ServiceResponse<string>(false, "No student data found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateStudentSetting(StudentSettingDTO studentSettingDto)
        {
            try
            {
                var response = await _studentInformationRepository.AddUpdateStudentSetting(studentSettingDto);
                return new ServiceResponse<int>(response.Success, response.Message, response.Data, response.StatusCode);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<List<StudentSettingDTO>>> GetStudentSettingByInstituteId(int instituteId)
        {
            try
            {
                var response = await _studentInformationRepository.GetStudentSettingByInstituteId(instituteId);
                if (response.Success)
                {
                    return new ServiceResponse<List<StudentSettingDTO>>(true, "Student setting retrieved successfully", response.Data, 200);
                }
                else
                {
                    return new ServiceResponse<List<StudentSettingDTO>>(false, response.Message, null, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentSettingDTO>>(false, ex.Message, null, 500);
            }
        }

    }
}
