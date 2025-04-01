using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Text;

namespace FeesManagement_API.Services.Implementations
{
    public class StudentFeeService : IStudentFeeService
    {
        private readonly IStudentFeeRepository _studentFeeRepository;

        public StudentFeeService(IStudentFeeRepository studentFeeRepository)
        {
            _studentFeeRepository = studentFeeRepository;
        }
         

        public ServiceResponse<List<StudentFeeResponse>> GetStudentFees(StudentFeeRequest request)
        {
            var studentFees = _studentFeeRepository.GetStudentFees(request);

            var totalCount = studentFees?.Count ?? 0; // Calculate total count if needed

            var response = new ServiceResponse<List<StudentFeeResponse>>(
                success: true,
                message: "Student fees retrieved successfully",
                data: studentFees,
                statusCode: 200,
                totalCount: totalCount
            );

            return response;
        }

        public ServiceResponse<DiscountStudentFeesResponse> DiscountStudentFees(DiscountStudentFeesRequest request)
        {
            // Insert discount record and get the DiscountID
            var discountID = _studentFeeRepository.DiscountStudentFees(request);

            var responseDto = new DiscountStudentFeesResponse
            {
                DiscountID = discountID,
                Message = "Discount applied successfully."
            };

            return new ServiceResponse<DiscountStudentFeesResponse>(
                success: true,
                message: "Discount applied successfully",
                data: responseDto,
                statusCode: 200
            );
        }

        public ServiceResponse<List<GetFeesChangeLogsResponse>> GetFeesChangeLogs(GetFeesChangeLogsRequest request)
        {
            var logs = _studentFeeRepository.GetFeesChangeLogs(request);
            var totalCount = logs?.Count ?? 0;
            return new ServiceResponse<List<GetFeesChangeLogsResponse>>(
                success: true,
                message: "Fees change logs retrieved successfully",
                data: logs,
                statusCode: 200,
                totalCount: totalCount
            );
        }
         
         
        public async Task<byte[]> GetStudentFeesExportAsync(GetStudentFeesExportRequest request)
        {
            // 1. Get raw fee data from the repository
            var rawData = await _studentFeeRepository.GetStudentFeeRawDataAsync(request);
            if (rawData == null || !rawData.Any())
                return Array.Empty<byte>();

           //2.Group and pivot data by student
           var pivotedData = rawData
               .GroupBy(x => new
               {
                   x.StudentID,
                   x.AdmissionNo,
                   x.StudentName,
                   x.RollNo,
                   x.ClassName,
                   x.SectionName,
                   x.ConcessionGroup
               })
               .Select(g =>
               {
                   var response = new DynamicStudentFeeResponse
                   {
                       AdmissionNo = g.First().AdmissionNo,
                       StudentName = g.First().StudentName,
                       RollNo = g.First().RollNo,
                       ClassName = g.First().ClassName,
                       SectionName = g.First().SectionName,
                       ConcessionGroup = g.First().ConcessionGroup,
                       TotalFeeAmount = g.Sum(x => x.FeeAmount)
                   };

                   // Build dynamic fee columns
                   foreach (var record in g)
                   {
                       string feeType = record.FeeType; // e.g., "Tuition Fee (Single)"
                       if (response.FeeAmounts.ContainsKey(feeType))
                           response.FeeAmounts[feeType] += record.FeeAmount;
                       else
                           response.FeeAmounts.Add(feeType, record.FeeAmount);
                   }

                   return response;
               })
               .ToList();
             

            // 3. Get distinct fee type columns (dynamic headers)
            var feeTypes = pivotedData
                .SelectMany(x => x.FeeAmounts.Keys)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            // 4. Generate output file based on ExportType
            return request.ExportType switch
            {
                1 => GenerateDynamicExcelFile(pivotedData, feeTypes),
                2 => GenerateDynamicCsvFile(pivotedData, feeTypes),
                _ => throw new ArgumentException("Invalid ExportType. Use 1 for Excel, 2 for CSV.")
            };
        }

        private byte[] GenerateDynamicExcelFile(List<DynamicStudentFeeResponse> data, List<string> feeTypes)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("StudentFees");
                int col = 1;
                // Fixed headers
                worksheet.Cells[1, col++].Value = "AdmissionNo";
                worksheet.Cells[1, col++].Value = "StudentName";
                worksheet.Cells[1, col++].Value = "RollNo";
                worksheet.Cells[1, col++].Value = "ClassName";
                worksheet.Cells[1, col++].Value = "SectionName";
                worksheet.Cells[1, col++].Value = "ConcessionGroup";
                worksheet.Cells[1, col++].Value = "TotalFeeAmount";

                // Dynamic fee type headers
                foreach (var feeType in feeTypes)
                    worksheet.Cells[1, col++].Value = feeType;

                int row = 2;
                foreach (var item in data)
                {
                    col = 1;
                    worksheet.Cells[row, col++].Value = item.AdmissionNo;
                    worksheet.Cells[row, col++].Value = item.StudentName;
                    worksheet.Cells[row, col++].Value = item.RollNo;
                    worksheet.Cells[row, col++].Value = item.ClassName;
                    worksheet.Cells[row, col++].Value = item.SectionName;
                    worksheet.Cells[row, col++].Value = item.ConcessionGroup;
                    worksheet.Cells[row, col++].Value = item.TotalFeeAmount;

                    // Write dynamic fee values; if missing, output 0.
                    foreach (var feeType in feeTypes)
                    {
                        decimal feeValue = item.FeeAmounts.ContainsKey(feeType) ? item.FeeAmounts[feeType] : 0;
                        worksheet.Cells[row, col++].Value = feeValue;
                    }
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        private byte[] GenerateDynamicCsvFile(List<DynamicStudentFeeResponse> data, List<string> feeTypes)
        {
            var sb = new StringBuilder();
            var headerColumns = new List<string>
            {
                "AdmissionNo", "StudentName", "RollNo", "ClassName", "SectionName", "ConcessionGroup", "TotalFeeAmount"
            };
            headerColumns.AddRange(feeTypes);
            sb.AppendLine(string.Join(",", headerColumns));

            foreach (var item in data)
            {
                var row = new List<string>
                {
                    SafeCsv(item.AdmissionNo),
                    SafeCsv(item.StudentName),
                    SafeCsv(item.RollNo),
                    SafeCsv(item.ClassName),
                    SafeCsv(item.SectionName),
                    SafeCsv(item.ConcessionGroup),
                    item.TotalFeeAmount.ToString()
                };

                foreach (var feeType in feeTypes)
                {
                    decimal feeValue = item.FeeAmounts.ContainsKey(feeType) ? item.FeeAmounts[feeType] : 0;
                    row.Add(feeValue.ToString());
                }

                sb.AppendLine(string.Join(",", row));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private string SafeCsv(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\r") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }










        public async Task<byte[]> GetFeesChangeLogsExportAsync(GetFeesChangeLogsExportRequest request)
        {
            var exportData = await _studentFeeRepository.GetFeesChangeLogsExportAsync(request);
            if (exportData == null || !exportData.Any())
                return Array.Empty<byte>();

            return request.ExportType switch
            {
                1 => GenerateFeesChangeLogsExcelFile(exportData),
                2 => GenerateFeesChangeLogsCsvFile(exportData),
                _ => throw new ArgumentException("Invalid ExportType. Use 1 for Excel, 2 for CSV.")
            };
        }

        private byte[] GenerateFeesChangeLogsExcelFile(IEnumerable<GetFeesChangeLogsExportResponse> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("FeesChangeLogs");
                int col = 1;

                // Header row (exact order as requested)
                ws.Cells[1, col++].Value = "AdmissionNumber";
                ws.Cells[1, col++].Value = "StudentName";
                ws.Cells[1, col++].Value = "RollNumber";
                ws.Cells[1, col++].Value = "ConcessionGroup";
                ws.Cells[1, col++].Value = "FeeHead";
                ws.Cells[1, col++].Value = "FeeTenurity";
                ws.Cells[1, col++].Value = "TotalFeeAmount";
                ws.Cells[1, col++].Value = "DiscountedAmount";
                ws.Cells[1, col++].Value = "DiscountedDateTime";
                ws.Cells[1, col++].Value = "UserName";

                int row = 2;
                foreach (var item in data)
                {
                    col = 1;
                    ws.Cells[row, col++].Value = item.AdmissionNumber;
                    ws.Cells[row, col++].Value = item.StudentName;
                    ws.Cells[row, col++].Value = item.RollNumber;
                    ws.Cells[row, col++].Value = item.ConcessionGroup;
                    ws.Cells[row, col++].Value = item.FeeHead;
                    ws.Cells[row, col++].Value = item.FeeTenurity;
                    ws.Cells[row, col++].Value = item.TotalFeeAmount;
                    ws.Cells[row, col++].Value = item.DiscountedAmount;
                    ws.Cells[row, col++].Value = item.DiscountedDateTime;
                    ws.Cells[row, col++].Value = item.UserName;
                    row++;
                }
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        private byte[] GenerateFeesChangeLogsCsvFile(IEnumerable<GetFeesChangeLogsExportResponse> data)
        {
            var sb = new StringBuilder();
            var headers = new List<string>
            {
                "AdmissionNumber",
                "StudentName",
                "RollNumber",
                "ConcessionGroup",
                "FeeHead",
                "FeeTenurity",
                "TotalFeeAmount",
                "DiscountedAmount",
                "DiscountedDateTime",
                "UserName"
            };
            sb.AppendLine(string.Join(",", headers));

            foreach (var item in data)
            {
                var row = new List<string>
                {
                    SafeCsv(item.AdmissionNumber),
                    SafeCsv(item.StudentName),
                    SafeCsv(item.RollNumber),
                    SafeCsv(item.ConcessionGroup),
                    SafeCsv(item.FeeHead),
                    SafeCsv(item.FeeTenurity),
                    item.TotalFeeAmount.ToString(),
                    item.DiscountedAmount.ToString(),
                    SafeCsv(item.DiscountedDateTime),
                    SafeCsv(item.UserName)
                };
                sb.AppendLine(string.Join(",", row));
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
