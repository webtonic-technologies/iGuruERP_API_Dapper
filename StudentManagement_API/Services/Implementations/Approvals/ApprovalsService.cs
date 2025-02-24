using CsvHelper;
using OfficeOpenXml;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using StudentManagement_API.Services.Interfaces;
using System.Globalization;
using System.Text;

namespace StudentManagement_API.Services.Implementations
{
    public class ApprovalsService : IApprovalsService
    {
        private readonly IApprovalsRepository _repository;

        public ApprovalsService(IApprovalsRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<int>> CreatePermissionSlipAsync(CreatePermissionSlipRequest request)
        {
            try
            {
                int id = await _repository.CreatePermissionSlipAsync(request);
                return new ServiceResponse<int>(true, "Permission slip created successfully.", id, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }


        public async Task<ServiceResponse<IEnumerable<GetPermissionSlipResponse>>> GetPermissionSlipAsync(GetPermissionSlipRequest request)
        {
            try
            {
                var data = await _repository.GetPermissionSlipAsync(request);
                return new ServiceResponse<IEnumerable<GetPermissionSlipResponse>>(
                    true,
                    "Permission slips retrieved successfully.",
                    data,
                    200,
                    data.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetPermissionSlipResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<bool>> ChangePermissionSlipStatusAsync(ChangePermissionSlipStatusRequest request)
        {
            try
            {
                bool result = await _repository.ChangePermissionSlipStatusAsync(request);
                if (result)
                {
                    return new ServiceResponse<bool>(true, "Permission slip status updated successfully.", result, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Permission slip not found or update failed.", result, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetApprovedHistoryResponse>>> GetApprovedHistoryAsync(GetApprovedHistoryRequest request)
        {
            try
            {
                var data = await _repository.GetApprovedHistoryAsync(request);
                return new ServiceResponse<IEnumerable<GetApprovedHistoryResponse>>(
                    true,
                    "Approved history retrieved successfully.",
                    data,
                    200,
                    data.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetApprovedHistoryResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetRejectedHistoryResponse>>> GetRejectedHistoryAsync(GetRejectedHistoryRequest request)
        {
            try
            {
                var data = await _repository.GetRejectedHistoryAsync(request);
                return new ServiceResponse<IEnumerable<GetRejectedHistoryResponse>>(
                    true,
                    "Rejected history retrieved successfully.",
                    data,
                    200,
                    data.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetRejectedHistoryResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }


        public async Task<ServiceResponse<string>> GetPermissionSlipExportAsync(GetPermissionSlipExportRequest request)
        {
            // Retrieve export data
            var data = await _repository.GetPermissionSlipExportAsync(request);
            if (data == null || !data.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            // Define file path based on export type
            string filePath = string.Empty;
            if (request.ExportType == 1)
            {
                // Generate Excel file using EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("PermissionSlipExport");
                    // Load data into worksheet (header row is automatically generated)
                    worksheet.Cells["A1"].LoadFromCollection(data, true);
                    // Auto-fit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "PermissionSlipExport.xlsx");
                    // Save to file
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                }
            }
            else if (request.ExportType == 2)
            {
                // Generate CSV file using CsvHelper
                var sb = new StringBuilder();
                using (var writer = new StringWriter(sb))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(data);
                    writer.Flush();
                }
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "PermissionSlipExport.csv");
                File.WriteAllText(filePath, sb.ToString());
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
            }

            return new ServiceResponse<string>(true, "Export file generated", filePath, 200);
        }

        public async Task<ServiceResponse<string>> GetApprovedHistoryExportAsync(GetApprovedHistoryExportRequest request)
        {
            // Retrieve export data
            var data = await _repository.GetApprovedHistoryExportAsync(request);
            if (data == null || !data.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            string filePath = string.Empty;
            // Create a memory stream to hold the exported file
            if (request.ExportType == 1)
            {
                // Export to Excel using EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("ApprovedHistoryExport");
                    // Load data into the worksheet (headers included)
                    worksheet.Cells["A1"].LoadFromCollection(data, true);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "ApprovedHistoryExport.xlsx");
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
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "ApprovedHistoryExport.csv");
                File.WriteAllText(filePath, sb.ToString());
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
            }

            return new ServiceResponse<string>(true, "Export file generated", filePath, 200);
        }

        public async Task<ServiceResponse<string>> GetRejectedHistoryExportAsync(GetRejectedHistoryExportRequest request)
        {
            // Retrieve export data from the repository.
            var data = await _repository.GetRejectedHistoryExportAsync(request);
            if (data == null || !data.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            string filePath = string.Empty;
            if (request.ExportType == 1)
            {
                // Generate Excel file using EPPlus.
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("RejectedHistoryExport");
                    // Load the data into the worksheet (headers included).
                    worksheet.Cells["A1"].LoadFromCollection(data, true);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "RejectedHistoryExport.xlsx");
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                }
            }
            else if (request.ExportType == 2)
            {
                // Generate CSV file using CsvHelper.
                var sb = new StringBuilder();
                using (var writer = new StringWriter(sb))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(data);
                    writer.Flush();
                }
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "RejectedHistoryExport.csv");
                File.WriteAllText(filePath, sb.ToString());
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
            }

            return new ServiceResponse<string>(true, "Export file generated", filePath, 200);
        }
    }
}
