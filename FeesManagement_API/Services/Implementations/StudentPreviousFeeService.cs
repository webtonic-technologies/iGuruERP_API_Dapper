using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Text;

namespace FeesManagement_API.Services.Implementations
{
    public class StudentPreviousFeeService : IStudentPreviousFeeService
    {
        private readonly IStudentPreviousFeeRepository _studentPreviousFeeRepository;

        public StudentPreviousFeeService(IStudentPreviousFeeRepository studentPreviousFeeRepository)
        {
            _studentPreviousFeeRepository = studentPreviousFeeRepository;
        }
         

        public ServiceResponse<List<StudentPreviousFeeResponse>> GetStudentPreviousFees(StudentPreviousFeeRequest request)
        {
            var studentFees = _studentPreviousFeeRepository.GetStudentPreviousFees(request);

            var totalCount = studentFees?.Count ?? 0; // Calculate total count if needed

            var response = new ServiceResponse<List<StudentPreviousFeeResponse>>(
                success: true,
                message: "Student fees retrieved successfully",
                data: studentFees,
                statusCode: 200,
                totalCount: totalCount
            );

            return response;
        }

        public ServiceResponse<IList<DiscountStudentPreviousFeesResponse>> DiscountStudentPreviousFees(
    IEnumerable<DiscountStudentPreviousFeesRequest> requests)
        {
            // call repository bulk‐insert and get back one ID per request
            var ids = _studentPreviousFeeRepository.DiscountStudentPreviousFees(requests);

            // build a response list
            var responses = requests
                .Zip(ids, (req, id) => new DiscountStudentPreviousFeesResponse
                {
                    DiscountID = id,
                    Message = $"Discount for Student {req.StudentID} applied."
                })
                .ToList();

            return new ServiceResponse<IList<DiscountStudentPreviousFeesResponse>>(
                success: true,
                message: "All discounts applied successfully",
                data: responses,
                statusCode: 200
            );
        }
         
        public ServiceResponse<List<GetPreviousFeesChangeLogsResponse>> GetPreviousFeesChangeLogs(GetPreviousFeesChangeLogsRequest request)
        {
            var logs = _studentPreviousFeeRepository.GetPreviousFeesChangeLogs(request);
            var totalCount = logs?.Count ?? 0;
            return new ServiceResponse<List<GetPreviousFeesChangeLogsResponse>>(
                success: true,
                message: "Fees change logs retrieved successfully",
                data: logs,
                statusCode: 200,
                totalCount: totalCount
            );
        }
         
         
        public async Task<byte[]> GetStudentPreviousFeesExportAsync(GetStudentPreviousFeesExportRequest request)
        {
            // 1. Get raw fee data from the repository
            var rawData = await _studentPreviousFeeRepository.GetStudentPreviousFeeRawDataAsync(request);
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
                   var response = new DynamicStudentPreviousFeeResponse
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
                1 => GeneratePreviousDynamicExcelFile(pivotedData, feeTypes),
                2 => GeneratePreviousDynamicCsvFile(pivotedData, feeTypes),
                _ => throw new ArgumentException("Invalid ExportType. Use 1 for Excel, 2 for CSV.")
            };
        }

        private byte[] GeneratePreviousDynamicExcelFile(List<DynamicStudentPreviousFeeResponse> data, List<string> feeTypes)
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

        private byte[] GeneratePreviousDynamicCsvFile(List<DynamicStudentPreviousFeeResponse> data, List<string> feeTypes)
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










        public async Task<byte[]> GetPreviousFeesChangeLogsExportAsync(GetPreviousFeesChangeLogsExportRequest request)
        {
            var exportData = await _studentPreviousFeeRepository.GetPreviousFeesChangeLogsExportAsync(request);
            if (exportData == null || !exportData.Any())
                return Array.Empty<byte>();

            return request.ExportType switch
            {
                1 => GeneratePreviousFeesChangeLogsExcelFile(exportData),
                2 => GeneratePreviousFeesChangeLogsCsvFile(exportData),
                _ => throw new ArgumentException("Invalid ExportType. Use 1 for Excel, 2 for CSV.")
            };
        }

        private byte[] GeneratePreviousFeesChangeLogsExcelFile(IEnumerable<GetPreviousFeesChangeLogsExportResponse> data)
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

        private byte[] GeneratePreviousFeesChangeLogsCsvFile(IEnumerable<GetPreviousFeesChangeLogsExportResponse> data)
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
