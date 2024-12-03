using iText.Commons.Utils;
using OfficeOpenXml;
using QRCoder;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.Responses;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Metadata;
using System.Text.Json;
using static iText.Kernel.Pdf.Colorspace.PdfPattern;
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

        public async Task<ServiceResponse<IEnumerable<StudentActivityHistoryResponse>>> GetStudentActivityHistory(int studentId, int instituteId)
        {
            try
            {
                // Fetch the activity history from the repository (returns IEnumerable<StudentActivityHistoryResponse>)
                var result = await _studentInformationRepository.GetStudentActivityHistory(studentId, instituteId);

                if (result != null)
                {
                    return new ServiceResponse<IEnumerable<StudentActivityHistoryResponse>>(true, "Activity history fetched successfully", result, 200);
                }

                return new ServiceResponse<IEnumerable<StudentActivityHistoryResponse>>(false, "No activity found for this student", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<StudentActivityHistoryResponse>>(false, ex.Message, null, 500);
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

        public async Task<ServiceResponse<List<dynamic>>> GetAllStudentDetailsData1(GetStudentRequestModel obj)
        {

            try
            {
                return await _studentInformationRepository.GetAllStudentDetailsData1(obj);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<dynamic>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> GetAllStudentDetailsAsExcelOld(getStudentRequest obj)
        {
            try
            {
                GetStudentRequestModel model = new GetStudentRequestModel();
                model.AcademicYearCode = obj.AcademicYearCode;
                model.section_id = obj.section_id;
                model.class_id = obj.class_id;
                model.Institute_id = obj.Institute_id;
                //model.isActive = obj.isActive;
                model.pageSize = int.MaxValue;
                model.pageNumber = 1;
                model.sortField = null;
                model.sortDirection = null;

                // Call the existing method to get the data
                var studentDetailsResponse = await _studentInformationRepository.GetAllStudentDetails(model);

                if (studentDetailsResponse.Data.Any())
                {
                    var fileName = $"StudentDetails_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
                    Directory.CreateDirectory(directoryPath);

                    // Check export format: 1 for Excel, 2 for CSV
                    if (obj.exportFormat == 1)
                    {
                        // Generate Excel file using EPPlus
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage())
                        {
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
                            var rowIndex = 2;
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

                            worksheet.Cells.AutoFitColumns();
                            var excelFile = package.GetAsByteArray();
                            var excelFilePath = Path.Combine(directoryPath, $"{fileName}.xlsx");

                            await File.WriteAllBytesAsync(excelFilePath, excelFile);
                            return new ServiceResponse<string>(true, "Excel file generated successfully", excelFilePath, 200);
                        }
                    }
                    else if (obj.exportFormat == 2)
                    {
                        // Generate CSV file
                        var csvFilePath = Path.Combine(directoryPath, $"{fileName}.csv");
                        var csvLines = new List<string>
                {
                    "Student Name,Class,Section,Admission Number,Roll Number,Date of Joining,Date of Birth,Religion,Gender,Father's Name"
                };

                        foreach (var student in studentDetailsResponse.Data)
                        {
                            var csvRow = string.Join(",",
                                student.Student_Name,
                                student.class_course,
                                student.Section,
                                student.Admission_Number,
                                student.Roll_Number,
                                student.Date_of_Joining,
                                student.Date_of_Birth,
                                student.Religion_Type,
                                student.Gender_Type,
                                student.Father_Name);
                            csvLines.Add(csvRow);
                        }

                        await File.WriteAllLinesAsync(csvFilePath, csvLines);
                        return new ServiceResponse<string>(true, "CSV file generated successfully", csvFilePath, 200);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Invalid export format", null, 400);
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

        public async Task<ServiceResponse<string>> GetAllStudentDetailsAsExcel1(getStudentRequest obj)
        {
            try
            {
                GetStudentRequestModel model = new GetStudentRequestModel
                {
                    AcademicYearCode = obj.AcademicYearCode,
                    section_id = obj.section_id,
                    class_id = obj.class_id,
                    Institute_id = obj.Institute_id,
                    //isActive = obj.isActive,
                    pageSize = int.MaxValue,
                    pageNumber = 1,
                    sortField = null,
                    sortDirection = null
                };

                // Call the existing method to get the data
                var studentDetailsResponse = await _studentInformationRepository.GetAllStudentDetailsData1(model);

                if (studentDetailsResponse.Data.Any())
                {
                    var fileName = $"StudentDetails_{DateTime.Now:yyyyMMddHHmmss}";
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
                    Directory.CreateDirectory(directoryPath);

                    // Check export format: 1 for Excel, 2 for CSV
                    if (obj.exportFormat == 1)
                    {
                        // Generate Excel file using EPPlus
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("StudentDetails");

                            // Get the first item to determine the headers
                            dynamic firstStudent = studentDetailsResponse.Data.First();
                            var properties = ((IDictionary<string, object>)firstStudent).Keys.ToList();

                            // Add headers
                            for (int i = 0; i < properties.Count; i++)
                            {
                                worksheet.Cells[1, i + 1].Value = properties[i];
                            }

                            // Add data rows dynamically
                            var rowIndex = 2;
                            foreach (dynamic student in studentDetailsResponse.Data)
                            {
                                var studentDict = (IDictionary<string, object>)student;
                                for (int i = 0; i < properties.Count; i++)
                                {
                                    if (studentDict.TryGetValue(properties[i], out var value))
                                    {
                                        worksheet.Cells[rowIndex, i + 1].Value = value;
                                    }
                                }
                                rowIndex++;
                            }

                            worksheet.Cells.AutoFitColumns();
                            var excelFile = package.GetAsByteArray();
                            var excelFilePath = Path.Combine(directoryPath, $"{fileName}.xlsx");

                            await File.WriteAllBytesAsync(excelFilePath, excelFile);
                            return new ServiceResponse<string>(true, "Excel file generated successfully", excelFilePath, 200);
                        }
                    }
                    else if (obj.exportFormat == 2)
                    {
                        // Generate CSV file
                        var csvFilePath = Path.Combine(directoryPath, $"{fileName}.csv");
                        var csvLines = new List<string>
                {
                    string.Join(",", ((IDictionary<string, object>)studentDetailsResponse.Data.First()).Keys) // Use keys as headers
                };

                        foreach (dynamic student in studentDetailsResponse.Data)
                        {
                            var studentDict = (IDictionary<string, object>)student;
                            var csvRow = string.Join(",", studentDict.Values.Select(value => value?.ToString().Replace(",", ";"))); // Replace commas in values
                            csvLines.Add(csvRow);
                        }

                        await File.WriteAllLinesAsync(csvFilePath, csvLines);
                        return new ServiceResponse<string>(true, "CSV file generated successfully", csvFilePath, 200);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Invalid export format", null, 400);
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


        public async Task<ServiceResponse<string>> GetAllStudentDetailsAsExcel(getStudentRequest obj)
        {
            try
            {
                GetStudentRequestModel model = new GetStudentRequestModel
                {
                    AcademicYearCode = obj.AcademicYearCode,
                    section_id = obj.section_id,
                    class_id = obj.class_id,
                    Institute_id = obj.Institute_id,
                    //isActive = obj.isActive,
                    pageSize = int.MaxValue,
                    pageNumber = 1,
                    sortField = null,
                    sortDirection = null
                };

                // Call the existing method to get the data
                var studentDetailsResponse = await _studentInformationRepository.GetAllStudentDetailsData1(model);

                if (studentDetailsResponse.Data.Any())
                {
                    var fileName = $"StudentDetails_{DateTime.Now:yyyyMMddHHmmss}";
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
                    Directory.CreateDirectory(directoryPath);

                    // Check export format: 1 for Excel, 2 for CSV
                    if (obj.exportFormat == 1)
                    {
                        // Generate Excel file using EPPlus
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("StudentDetails");

                            // Get the first item to determine the headers
                            dynamic firstStudent = studentDetailsResponse.Data.First();
                            var properties = ((IDictionary<string, object>)firstStudent).Keys.ToList();

                            // Prepare a list to hold dynamic headers
                            var headers = new List<string>(properties);

                            // Add dynamic sibling headers if SiblingInfo exists
                            foreach (dynamic student in studentDetailsResponse.Data)
                            {
                                if (student.SiblingInfo != null)
                                {
                                    var siblings = JsonSerializer.Deserialize<List<StudentSiblings>>(student.SiblingInfo.ToString());
                                    for (int i = 0; i < siblings.Count; i++)
                                    {
                                        // Construct sibling header names
                                        var siblingNameHeader = $"sibling{i + 1}_Name";
                                        var siblingMiddleNameHeader = $"sibling{i + 1}_Middle_Name";
                                        var siblingLastNameHeader = $"sibling{i + 1}_Last_Name";
                                        var siblingClassHeader = $"sibling{i + 1}_Class";
                                        var siblingSectionHeader = $"sibling{i + 1}_Section";
                                        var siblingDOBHeader = $"sibling{i + 1}_DateOfBirth";
                                        var siblingAadharNoHeader = $"sibling{i + 1}_AadharNo";

                                        // Check if the headers are already in the list before adding
                                        if (!headers.Contains(siblingNameHeader)) headers.Add(siblingNameHeader);
                                        if (!headers.Contains(siblingMiddleNameHeader)) headers.Add(siblingMiddleNameHeader);
                                        if (!headers.Contains(siblingLastNameHeader)) headers.Add(siblingLastNameHeader);
                                        if (!headers.Contains(siblingClassHeader)) headers.Add(siblingClassHeader);
                                        if (!headers.Contains(siblingSectionHeader)) headers.Add(siblingSectionHeader);
                                        if (!headers.Contains(siblingDOBHeader)) headers.Add(siblingDOBHeader);
                                        if (!headers.Contains(siblingAadharNoHeader)) headers.Add(siblingAadharNoHeader);

                                    }
                                }
                            }
                            headers.Remove("SiblingInfo");
                            // Add headers to worksheet
                            for (int i = 0; i < headers.Count; i++)
                            {
                                worksheet.Cells[1, i + 1].Value = headers[i];
                            }

                            // Add data rows dynamically
                            var rowIndex = 2;
                            foreach (dynamic student in studentDetailsResponse.Data)
                            {
                                var studentDict = (IDictionary<string, object>)student;

                                // Add student details to the row
                                for (int i = 0; i < properties.Count; i++)
                                {
                                    if (studentDict.TryGetValue(properties[i], out var value) )
                                    {
                                        if(properties[i] != "SiblingInfo")
                                        {
                                            worksheet.Cells[rowIndex, i + 1].Value = value;
                                        }
                                     
                                    }
                                }

                                // Add sibling details if present
                                if (student.SiblingInfo != null)
                                {
                                    var count = properties.Count;
                                    var siblings = JsonSerializer.Deserialize<List<StudentSiblings>>(student.SiblingInfo.ToString());
                                    for (int i = 0; i < siblings.Count; i++)
                                    {
                                       
                                        if (i == 0)
                                        {
                                            count = properties.Count - 1;
                                        }
                                        
                                        worksheet.Cells[rowIndex, count + i * 6 + 1].Value = siblings[i].Name;
                                        worksheet.Cells[rowIndex, count + i * 6 + 2].Value = siblings[i].Middle_Name;
                                        worksheet.Cells[rowIndex, count + i * 6 + 3].Value = siblings[i].Last_Name;
                                        worksheet.Cells[rowIndex, count + i * 6 + 4].Value = siblings[i].Class;
                                        worksheet.Cells[rowIndex, count + i * 6 + 5].Value = siblings[i].section;
                                        worksheet.Cells[rowIndex, count + i * 6 + 6].Value = siblings[i].Date_of_Birth;
                                        worksheet.Cells[rowIndex, count + i * 6 + 7].Value = siblings[i].Aadhar_no;
                                    }
                                }

                                rowIndex++;
                            }

                            worksheet.Cells.AutoFitColumns();
                            var excelFile = package.GetAsByteArray();
                            var excelFilePath = Path.Combine(directoryPath, $"{fileName}.xlsx");

                            await File.WriteAllBytesAsync(excelFilePath, excelFile);
                            return new ServiceResponse<string>(true, "Excel file generated successfully", excelFilePath, 200);
                        }
                    }
                    else if (obj.exportFormat == 2)
                    {
                        // Generate CSV file
                        var csvFilePath = Path.Combine(directoryPath, $"{fileName}.csv");
                        var csvLines = new List<string>
                {
                    string.Join(",", ((IDictionary<string, object>)studentDetailsResponse.Data.First()).Keys) // Use keys as headers
                };

                        foreach (dynamic student in studentDetailsResponse.Data)
                        {
                            var studentDict = (IDictionary<string, object>)student;
                            var csvRow = string.Join(",", studentDict.Values.Select(value => value?.ToString().Replace(",", ";"))); // Replace commas in values
                            csvLines.Add(csvRow);
                        }

                        await File.WriteAllLinesAsync(csvFilePath, csvLines);
                        return new ServiceResponse<string>(true, "CSV file generated successfully", csvFilePath, 200);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Invalid export format", null, 400);
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
