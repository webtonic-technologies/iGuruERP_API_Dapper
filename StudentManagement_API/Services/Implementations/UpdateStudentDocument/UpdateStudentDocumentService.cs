using OfficeOpenXml;
using StudentManagement_API.DTOs.Requests; 
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using StudentManagement_API.Services.Interfaces;
using System.Text;

namespace StudentManagement_API.Services.Implementations
{
    public class UpdateStudentDocumentService : IUpdateStudentDocumentService
    {
        private readonly IUpdateStudentDocumentRepository _repository;

        public UpdateStudentDocumentService(IUpdateStudentDocumentRepository repository)
        {
            _repository = repository;
        }
         

        public async Task<ServiceResponse<List<int>>> AddUpdateDocumentAsync(AddUpdateDocumentRequest request)
        {
            try
            {
                var ids = await _repository.AddDocumentAsync(request);
                return new ServiceResponse<List<int>>(true, "Documents added successfully.", ids, 200, ids.Count);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<int>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetDocumentsResponse>>> GetDocumentsAsync(GetDocumentsRequest request)
        {
            try
            {
                var documents = await _repository.GetDocumentsAsync(request);
                return new ServiceResponse<IEnumerable<GetDocumentsResponse>>(
                    true,
                    "Documents retrieved successfully.",
                    documents,
                    200,
                    documents.Count()
                );
            }
            catch (Exception ex)
            {
                // Log the exception as needed.
                return new ServiceResponse<IEnumerable<GetDocumentsResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteDocumentAsync(DeleteDocumentRequest request)
        {
            try
            {
                bool deleted = await _repository.DeleteDocumentAsync(request);
                if (deleted)
                {
                    return new ServiceResponse<bool>(true, "Document deleted successfully.", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Document not found or could not be deleted.", false, 404);
                }
            }
            catch (Exception ex)
            {
                // Log the exception as needed.
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<bool>> SetDocumentManagerAsync(List<SetDocumentManagerRequest> requests)
        {
            try
            {
                bool result = await _repository.SetDocumentManagerAsync(requests);
                return new ServiceResponse<bool>(true, "Document manager settings updated successfully.", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetDocumentManagerResponse>>> GetDocumentManagerAsync(GetDocumentManagerRequest request)
        {
            try
            {
                var data = await _repository.GetDocumentManagerAsync(request);
                return new ServiceResponse<IEnumerable<GetDocumentManagerResponse>>(
                    true,
                    "Document manager data retrieved successfully.",
                    data,
                    200,
                    data.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetDocumentManagerResponse>>(
                    false,
                    ex.Message,
                    null,
                    500);
            }
        }

        public async Task<ServiceResponse<string>> GetDocumentManagerExportAsync(GetDocumentManagerExportRequest request)
        {
            // 1. Fetch grouped data (one record per student, with a Documents list).
            var studentData = await _repository.GetDocumentManagerExportAsync(request);
            if (studentData == null || !studentData.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            // 2. Determine all distinct documents across all students
            //    We'll sort them by DocumentName for consistent column order.
            var allDocumentNames = studentData
                .SelectMany(s => s.Documents)
                .Select(d => d.DocumentName)
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            // 3. Export based on request.ExportType
            if (request.ExportType == 1)
            {
                // Generate Excel
                var filePath = GenerateExcelFile(studentData, allDocumentNames);
                return new ServiceResponse<string>(true, "Excel file generated", filePath, 200);
            }
            else if (request.ExportType == 2)
            {
                // Generate CSV
                var filePath = GenerateCsvFile(studentData, allDocumentNames);
                return new ServiceResponse<string>(true, "CSV file generated", filePath, 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType", null, 400);
            }
        }

        private string GenerateExcelFile(
            IEnumerable<GetDocumentManagerExportResponse> studentData,
            List<string> allDocumentNames)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DocumentManager");

                // 1. Header Row
                // Student columns
                worksheet.Cells[1, 1].Value = "Student Name";
                worksheet.Cells[1, 2].Value = "Admission Number";
                worksheet.Cells[1, 3].Value = "Class";
                worksheet.Cells[1, 4].Value = "Section";

                // Document columns
                int docStartCol = 5;
                for (int i = 0; i < allDocumentNames.Count; i++)
                {
                    worksheet.Cells[1, docStartCol + i].Value = allDocumentNames[i];
                }

                // 2. Populate rows (one row per student)
                int currentRow = 2;
                foreach (var student in studentData)
                {
                    worksheet.Cells[currentRow, 1].Value = student.StudentName;
                    worksheet.Cells[currentRow, 2].Value = student.AdmissionNumber;
                    worksheet.Cells[currentRow, 3].Value = student.Class;
                    worksheet.Cells[currentRow, 4].Value = student.Section;

                    // Create a quick lookup: DocumentName => IsSubmitted
                    var docLookup = student.Documents.ToDictionary(d => d.DocumentName, d => d.IsSubmitted);

                    // Fill each Document column with "Yes" or "No"
                    for (int i = 0; i < allDocumentNames.Count; i++)
                    {
                        var docName = allDocumentNames[i];
                        bool isSubmitted = docLookup.ContainsKey(docName) && docLookup[docName];
                        worksheet.Cells[currentRow, docStartCol + i].Value = isSubmitted ? "Yes" : "No";
                    }

                    currentRow++;
                }

                // (Optional) Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // 3. Save to file on disk (same approach as your reference code)
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "DocumentManagerExport.xlsx");
                var fileBytes = package.GetAsByteArray();
                File.WriteAllBytes(filePath, fileBytes);

                return filePath;
            }
        }

        private string GenerateCsvFile(
            IEnumerable<GetDocumentManagerExportResponse> studentData,
            List<string> allDocumentNames)
        {
            // Build CSV lines manually or use CSVHelper
            var sb = new StringBuilder();

            // 1. Header row
            sb.Append("Student Name,Admission Number,Class,Section");
            foreach (var docName in allDocumentNames)
            {
                sb.Append($",{docName}");
            }
            sb.AppendLine();

            // 2. Rows
            foreach (var student in studentData)
            {
                // Basic student columns
                sb.Append($"{EscapeCsv(student.StudentName)},{EscapeCsv(student.AdmissionNumber)},{EscapeCsv(student.Class)},{EscapeCsv(student.Section)}");

                // Document columns
                var docLookup = student.Documents.ToDictionary(d => d.DocumentName, d => d.IsSubmitted);
                foreach (var docName in allDocumentNames)
                {
                    bool isSubmitted = docLookup.ContainsKey(docName) && docLookup[docName];
                    sb.Append($",{(isSubmitted ? "Yes" : "No")}");
                }
                sb.AppendLine();
            }

            // 3. Write to disk
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "DocumentManagerExport.csv");
            File.WriteAllText(filePath, sb.ToString());
            return filePath;
        }

        // Simple CSV field escaping (handles commas, quotes, etc.)
        private string EscapeCsv(string field)
        {
            if (field == null) return "";
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\r") || field.Contains("\n"))
            {
                // Escape quotes by doubling them
                field = field.Replace("\"", "\"\"");
                // Wrap in quotes
                field = $"\"{field}\"";
            }
            return field;
        }
    }
}
