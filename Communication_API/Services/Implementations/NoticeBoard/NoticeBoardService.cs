using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.DTOs.Responses.NoticeBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.NoticeBoard;
using Communication_API.Repository.Interfaces.NoticeBoard;
using Communication_API.Services.Interfaces.NoticeBoard;
using OfficeOpenXml;
using System.Text;

namespace Communication_API.Services.Implementations.NoticeBoard
{
    public class NoticeBoardService : INoticeBoardService
    {
        private readonly INoticeBoardRepository _noticeBoardRepository;

        public NoticeBoardService(INoticeBoardRepository noticeBoardRepository)
        {
            _noticeBoardRepository = noticeBoardRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateNotice(AddUpdateNoticeRequest request)
        {
            return await _noticeBoardRepository.AddUpdateNotice(request);
        }

        public async Task<ServiceResponse<List<NoticeResponse>>> GetAllNotice(GetAllNoticeRequest request)
        {
            return await _noticeBoardRepository.GetAllNotice(request);
        }


        public async Task<ServiceResponse<string>> AddUpdateCircular(AddUpdateCircularRequest request)
        {
            return await _noticeBoardRepository.AddUpdateCircular(request);
        }

        public async Task<ServiceResponse<List<CircularResponse>>> GetAllCircular(GetAllCircularRequest request)
        {
            return await _noticeBoardRepository.GetAllCircular(request);
        }

        public async Task<ServiceResponse<string>> NoticeSetStudentView(NoticeSetStudentViewRequest request)
        {
            return await _noticeBoardRepository.NoticeSetStudentView(request);
        }
        public async Task<ServiceResponse<string>> NoticeSetEmployeeView(NoticeSetEmployeeViewRequest request)
        {
            return await _noticeBoardRepository.NoticeSetEmployeeView(request);
        }

        public async Task<ServiceResponse<StudentNoticeStatisticsResponse>> GetStudentNoticeStatistics(GetStudentNoticeStatisticsRequest request)
        {
            return await _noticeBoardRepository.GetStudentNoticeStatistics(request);
        }

        public async Task<ServiceResponse<EmployeeNoticeStatisticsResponse>> GetEmployeeNoticeStatistics(GetEmployeeNoticeStatisticsRequest request)
        {
            return await _noticeBoardRepository.GetEmployeeNoticeStatistics(request);
        }
        public async Task<ServiceResponse<string>> DeleteNotice(int InstituteID, int NoticeID)
        {
            return await _noticeBoardRepository.DeleteNotice(InstituteID, NoticeID);
        }
        public async Task<ServiceResponse<string>> DeleteCircular(DeleteCircularRequest request)
        {
            return await _noticeBoardRepository.DeleteCircular(request);
        }
        public async Task<ServiceResponse<string>> CircularSetStudentView(CircularSetStudentViewRequest request)
        {
            return await _noticeBoardRepository.CircularSetStudentView(request);
        }
        public async Task<ServiceResponse<string>> CircularSetEmployeeView(CircularSetEmployeeViewRequest request)
        {
            return await _noticeBoardRepository.CircularSetEmployeeView(request);
        }
        public async Task<ServiceResponse<StudentCircularStatisticsResponse>> GetStudentCircularStatistics(GetStudentCircularStatisticsRequest request)
        {
            return await _noticeBoardRepository.GetStudentCircularStatistics(request);
        }
        public async Task<ServiceResponse<EmployeeCircularStatisticsResponse>> GetEmployeeCircularStatistics(GetEmployeeCircularStatisticsRequest request)
        {
            return await _noticeBoardRepository.GetEmployeeCircularStatistics(request);
        }

        public async Task<ServiceResponse<string>> GetAllNoticeExport(GetAllNoticeExportRequest request)
        {
            // Fetch the export data from the repository
            var exportData = await _noticeBoardRepository.GetAllNoticeExportData(request);
            if (exportData == null || !exportData.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            if (request.ExportType == 1)
            {
                // Generate Excel file
                var filePath = GenerateExcelFile(exportData);
                return new ServiceResponse<string>(true, "Excel file generated", filePath, 200);
            }
            else if (request.ExportType == 2)
            {
                // Generate CSV file
                var filePath = GenerateCsvFile(exportData);
                return new ServiceResponse<string>(true, "CSV file generated", filePath, 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
            }
        }

        private string GenerateExcelFile(List<GetAllNoticeExportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Notice Export");
                // Write headers
                worksheet.Cells[1, 1].Value = "Title";
                worksheet.Cells[1, 2].Value = "Description";
                worksheet.Cells[1, 3].Value = "Start Date";
                worksheet.Cells[1, 4].Value = "End Date";
                worksheet.Cells[1, 5].Value = "Recipients";
                worksheet.Cells[1, 6].Value = "Created On";
                worksheet.Cells[1, 7].Value = "Created By";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Title;
                    worksheet.Cells[row, 2].Value = item.Description;
                    worksheet.Cells[row, 3].Value = item.StartDate;
                    worksheet.Cells[row, 4].Value = item.EndDate;
                    worksheet.Cells[row, 5].Value = item.Recipients;
                    worksheet.Cells[row, 6].Value = item.CreatedOn;
                    worksheet.Cells[row, 7].Value = item.CreatedBy;
                    row++;
                }

                var fileBytes = package.GetAsByteArray();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "NoticeReport.xlsx");
                File.WriteAllBytes(filePath, fileBytes);
                return filePath;
            }
        }

        private string GenerateCsvFile(List<GetAllNoticeExportResponse> data)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Title,Description,Start Date,End Date,Recipients,Created On,Created By");

            foreach (var item in data)
            {
                // Optionally, escape commas in values if needed
                csvBuilder.AppendLine($"{item.Title},{item.Description},{item.StartDate},{item.EndDate},{item.Recipients},{item.CreatedOn},{item.CreatedBy}");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "NoticeReport.csv");
            File.WriteAllText(filePath, csvBuilder.ToString());
            return filePath;
        }

        public async Task<ServiceResponse<string>> GetAllCircularExport(GetAllCircularExportRequest request)
        {
            // Fetch the export data from the repository
            var exportData = await _noticeBoardRepository.GetAllCircularExportData(request);
            if (exportData == null || !exportData.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            // Generate file based on ExportType: 1 = Excel, 2 = CSV
            if (request.ExportType == 1)
            {
                var filePath = GenerateExcelFile(exportData);
                return new ServiceResponse<string>(true, "Excel file generated", filePath, 200);
            }
            else if (request.ExportType == 2)
            {
                var filePath = GenerateCsvFile(exportData);
                return new ServiceResponse<string>(true, "CSV file generated", filePath, 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
            }
        }

        private string GenerateExcelFile(List<GetAllCircularExportResponse> data)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Circular Export");

                // Write header row
                worksheet.Cells[1, 1].Value = "AcademicYear";
                worksheet.Cells[1, 2].Value = "CircularNo";
                worksheet.Cells[1, 3].Value = "Title";
                worksheet.Cells[1, 4].Value = "CircularDate";
                worksheet.Cells[1, 5].Value = "PublishedDate";
                worksheet.Cells[1, 6].Value = "Recipients";
                worksheet.Cells[1, 7].Value = "CreatedOn";
                worksheet.Cells[1, 8].Value = "CreatedBy";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.AcademicYear;
                    worksheet.Cells[row, 2].Value = item.CircularNo;
                    worksheet.Cells[row, 3].Value = item.Title;
                    worksheet.Cells[row, 4].Value = item.CircularDate;
                    worksheet.Cells[row, 5].Value = item.PublishedDate;
                    worksheet.Cells[row, 6].Value = item.Recipients;
                    worksheet.Cells[row, 7].Value = item.CreatedOn;
                    worksheet.Cells[row, 8].Value = item.CreatedBy;
                    row++;
                }

                var fileBytes = package.GetAsByteArray();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CircularReport.xlsx");
                File.WriteAllBytes(filePath, fileBytes);
                return filePath;
            }
        }

        private string GenerateCsvFile(List<GetAllCircularExportResponse> data)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("AcademicYear,CircularNo,Title,CircularDate,PublishedDate,Recipients,CreatedOn,CreatedBy");

            foreach (var item in data)
            {
                csvBuilder.AppendLine($"{item.AcademicYear},{item.CircularNo},{item.Title},{item.CircularDate},{item.PublishedDate},{item.Recipients},{item.CreatedOn},{item.CreatedBy}");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CircularReport.csv");
            File.WriteAllText(filePath, csvBuilder.ToString());
            return filePath;
        }
    }
}
