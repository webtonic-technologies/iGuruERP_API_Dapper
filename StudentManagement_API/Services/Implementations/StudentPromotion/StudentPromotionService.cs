using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Services.Interfaces;
using StudentManagement_API.Repository.Interfaces;
using OfficeOpenXml;
using System.Text;
using System.Globalization;
using CsvHelper;

namespace StudentManagement_API.Services.Implementations
{
    public class StudentPromotionService : IStudentPromotionService
    {
        private readonly IStudentPromotionRepository _repository;

        public StudentPromotionService(IStudentPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<IEnumerable<GetClassPromotionResponse>>> GetClassPromotionAsync(GetClassPromotionRequest request)
        {
            try
            {
                var data = await _repository.GetClassPromotionAsync(request);
                // If you need a count, make sure to materialize the enumerable to a list
                var dataList = data.ToList();
                return new ServiceResponse<IEnumerable<GetClassPromotionResponse>>(
                    true,
                    "Class promotions retrieved successfully.",
                    dataList,
                    200,
                    dataList.Count
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetClassPromotionResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<bool>> UpdateClassPromotionConfigurationAsync(UpdateClassPromotionConfigurationRequest request)
        {
            try
            {
                bool result = await _repository.UpdateClassPromotionConfigurationAsync(request);
                return new ServiceResponse<bool>(
                    true,
                    "Class promotion configuration updated successfully.",
                    result,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(
                    false,
                    ex.Message,
                    false,
                    500
                );
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetClassPromotionHistoryResponse>>> GetClassPromotionHistoryAsync(GetClassPromotionHistoryRequest request)
        {
            try
            {
                var (data, totalCount) = await _repository.GetClassPromotionHistoryAsync(request);
                return new ServiceResponse<IEnumerable<GetClassPromotionHistoryResponse>>(
                    true,
                    "Class promotion history retrieved successfully.",
                    data,
                    200,
                    totalCount
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetClassPromotionHistoryResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<string>> GetClassPromotionHistoryExportAsync(GetClassPromotionHistoryExportRequest request)
        {
            var data = await _repository.GetClassPromotionHistoryExportAsync(request);
            if (data == null || !data.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            string filePath = string.Empty;
            if (request.ExportType == 1)
            {
                // Export to Excel using EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("ClassPromotionHistoryExport");
                    worksheet.Cells["A1"].LoadFromCollection(data, true);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "ClassPromotionHistoryExport.xlsx");
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                }
            }
            else if (request.ExportType == 2)
            {
                // Export to CSV using CsvHelper
                var sb = new StringBuilder();
                using (var writer = new StringWriter(sb))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(data);
                    writer.Flush();
                }
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "ClassPromotionHistoryExport.csv");
                File.WriteAllText(filePath, sb.ToString());
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
            }

            return new ServiceResponse<string>(true, "Export file generated", filePath, 200);
        }

        public async Task<ServiceResponse<bool>> PromoteStudentsAsync(PromoteStudentsRequest request)
        {
            try
            {
                bool result = await _repository.PromoteStudentsAsync(request);
                return new ServiceResponse<bool>(
                    true,
                    "Students promoted successfully.",
                    result,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(
                    false,
                    ex.Message,
                    false,
                    500
                );
            }
        }


        public async Task<ServiceResponse<IEnumerable<GetClassesResponse>>> GetClassesAsync(GetClassesRequest request)
        {
            try
            {
                var data = await _repository.GetClassesAsync(request);
                return new ServiceResponse<IEnumerable<GetClassesResponse>>(
                    true,
                    "Classes retrieved successfully.",
                    data,
                    200,
                    data.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetClassesResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetSectionsResponse>>> GetSectionsAsync(GetSectionsRequest request)
        {
            try
            {
                var data = await _repository.GetSectionsAsync(request);
                return new ServiceResponse<IEnumerable<GetSectionsResponse>>(
                    true,
                    "Sections retrieved successfully.",
                    data,
                    200,
                    data.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetSectionsResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentsPromotionResponse>>> GetStudentsAsync(GetStudentsPromotionRequest request)
        {
            try
            {
                var data = await _repository.GetStudentsAsync(request);
                return new ServiceResponse<IEnumerable<GetStudentsPromotionResponse>>(
                    true,
                    "Students retrieved successfully.",
                    data,
                    200,
                    data.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetStudentsPromotionResponse>>(
                    false,
                    ex.Message,
                    null,
                    500);
            }
        }
    }
}
