using OfficeOpenXml;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using System.Drawing.Printing;

namespace Student_API.Services.Implementations
{
    public class StudentPromotionService : IStudentPromotionService
    {
        private readonly IStudentPromotionRepository _studentPromotionRepository;
        private readonly IImageService _imageService;
        private readonly ICommonService _commonService;
        public StudentPromotionService(IStudentPromotionRepository studentPromotionRepository, IImageService imageService,ICommonService commonService)
        {
            _studentPromotionRepository = studentPromotionRepository;
            _imageService = imageService;
            _commonService = commonService;
        }
        public async Task<ServiceResponse<List<StudentPromotionDTO>>> GetStudentsForPromotion(GetStudentsForPromotionParam obj)
        {
            try
            {
                var data = await _studentPromotionRepository.GetStudentsForPromotion(obj.classId, obj.sortField, obj.sortDirection, obj.pageSize, obj.pageNumber);
                if (data.Data != null)
                {
                    foreach (var item in data.Data)
                    {
                        if (!string.IsNullOrEmpty(item.File_Name) && File.Exists(item.File_Name))
                        {
                            item.File_Name = _imageService.GetImageAsBase64(item.File_Name);
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentPromotionDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> PromoteStudents(List<int> studentIds, int nextClassId, int sectionId, int CurrentAcademicYear)
        {
            try
            {
                var data = await _studentPromotionRepository.PromoteStudents(studentIds, nextClassId, sectionId,CurrentAcademicYear);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<bool>> PromoteClasses(ClassPromotionDTO classPromotionDTO)
        {
            try
            {
                var fromClassIds = new HashSet<int>();
                var toClassIds = new HashSet<int>();

                foreach (var classSection in classPromotionDTO.ClassSections)
                {
                    // Validate FromClassId
                    if (!fromClassIds.Add(classSection.OldClassId))
                    {
                        return new ServiceResponse<bool>(false, $"Duplicate FromClassId: {classSection.OldClassId}", false, 400);
                    }

                    // Validate ToClassId
                    if (!toClassIds.Add(classSection.NewClassId))
                    {
                        return new ServiceResponse<bool>(false, $"Duplicate ToClassId: {classSection.NewClassId}", false, 400);
                    }
                }

                var data = await _studentPromotionRepository.PromoteClasses(classPromotionDTO);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<int?>> GetToClassIdAsync(ClassPromotionParams classPromotionDTO)
        {

            var data = await _studentPromotionRepository.GetToClassIdAsync(classPromotionDTO.FromClassId,classPromotionDTO.InstituteId);
            return data;

        }
        public async Task<ServiceResponse<List<ClassPromotionLogDTO>>> GetClassPromotionLog(GetClassPromotionLogParam obj)
        {
            try
            {
                var data = await _studentPromotionRepository.GetClassPromotionLog(obj);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClassPromotionLogDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> ExportClassPromotionLogToExcel(ExportClassPromotionLogParam obj)
        {
            try
            {
                GetClassPromotionLogParam getClassPromotionLogParam = new GetClassPromotionLogParam();
                getClassPromotionLogParam.institute_id = obj.institute_id;
                getClassPromotionLogParam.pageNumber = 1;
                getClassPromotionLogParam.pageSize = int.MaxValue;
                getClassPromotionLogParam.sortField = "LogId";
                getClassPromotionLogParam.sortDirection = "ASC";

                // Fetch logs from the repository
                var logsResponse = await _studentPromotionRepository.GetClassPromotionLog(getClassPromotionLogParam);

                // Check if logs were retrieved successfully
                //if (!logsResponse.Success)
                //{
                //    return new ServiceResponse<string>(false, "No logs found", null, 404);
                //}

                var logs = logsResponse.Data;

                var headers = new List<string>
    {
        "Log ID", "User ID", "IP Address", "Promotion Date/Time"
    };

                // Call the generic export function
                return await _commonService.ExportDataToFile(logs, headers, obj.exportFormat, $"ClassPromotionLog_{DateTime.Now:yyyyMMddHHmmss}");

                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //// Create an Excel package using EPPlus
                //using (var package = new ExcelPackage())
                //{
                //    // Add a worksheet
                //    var worksheet = package.Workbook.Worksheets.Add("ClassPromotionLogs");

                //    // Add headers
                //    worksheet.Cells[1, 1].Value = "Log ID";
                //    worksheet.Cells[1, 2].Value = "User ID";
                //    worksheet.Cells[1, 3].Value = "IP Address";
                //    worksheet.Cells[1, 4].Value = "Promotion Date/Time";

                //    // Add data rows
                //    var rowIndex = 2; // Start from row 2 as row 1 contains headers
                //    foreach (var log in logs)
                //    {
                //        worksheet.Cells[rowIndex, 1].Value = log.LogId;
                //        worksheet.Cells[rowIndex, 2].Value = log.UserId;
                //        worksheet.Cells[rowIndex, 3].Value = log.IPAddress;
                //        worksheet.Cells[rowIndex, 4].Value = log.PromotionDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                //        rowIndex++;
                //    }

                //    // Auto-fit columns for better readability
                //    worksheet.Cells.AutoFitColumns();

                //    // Generate the Excel file as a byte array
                //    var excelFile = package.GetAsByteArray();

                //    // Save the file to a specific location or return the file content as a downloadable response
                //    var fileName = $"ClassPromotionLog_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);

                //    // Ensure the directory exists
                //    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                //    // Write file to disk
                //    await File.WriteAllBytesAsync(filePath, excelFile);

                //    // Return the file path as a response
                //    return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
                //}
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

    }
}
