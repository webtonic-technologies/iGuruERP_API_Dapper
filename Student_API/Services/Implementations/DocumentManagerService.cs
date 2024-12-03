using OfficeOpenXml;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Models;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;

namespace Student_API.Services.Implementations
{
    public class DocumentManagerService : IDocumentManagerService
    {
        private readonly IDocumentManagerRepository _documentManagerRepository;

        public DocumentManagerService(IDocumentManagerRepository documentManagerRepository)
        {
            _documentManagerRepository = documentManagerRepository;
        }

        public async Task<ServiceResponse<List<StudentDocumentInfo>>> GetStudentDocuments(int Institute_id, int classId, int sectionId, string sortColumn, string sortDirection, int? pageSize, int? pageNumber, string searchQuery)
        {
            return await _documentManagerRepository.GetStudentDocuments(Institute_id,classId, sectionId,sortColumn ,sortDirection,pageSize, pageNumber,searchQuery);
        }

        public async Task<ServiceResponse<bool>> UpdateStudentDocumentStatuses(List<DocumentUpdateRequest> updates)
        {
            return await _documentManagerRepository.UpdateStudentDocumentStatuses(updates);
        }

        public async Task<ServiceResponse<string>> ExportStudentDocuments(int Institute_id, int classId, int sectionId, string sortColumn, string sortDirection, int? pageSize, int? pageNumber, int exportFormat)
        {
            // Fetch data from the repository
            var data = await _documentManagerRepository.GetStudentDocuments(Institute_id, classId, sectionId, sortColumn, sortDirection, pageSize, pageNumber, "");
            var studentDocuments = data.Data;

            if (studentDocuments == null || !studentDocuments.Any())
            {
                return new ServiceResponse<string>(false, "No student documents found", null, 404);
            }

            // Directory setup for storing files
            var fileName = $"StudentDocuments_{DateTime.Now:yyyyMMddHHmmss}";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
            Directory.CreateDirectory(directoryPath);

            if (exportFormat == 1) // Excel Export
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("StudentDocuments");

                    // Add fixed headers
                    worksheet.Cells[1, 1].Value = "Sr no";
                    worksheet.Cells[1, 2].Value = "Student Name";
                    worksheet.Cells[1, 3].Value = "Admission Number";
                    worksheet.Cells[1, 4].Value = "Class Name";
                    worksheet.Cells[1, 5].Value = "Section Name";

                    // Dynamically add document columns based on the document names
                    var docColumns = studentDocuments.SelectMany(s => s.DocumentStatus.Keys).Distinct().ToList();
                    for (int i = 0; i < docColumns.Count; i++)
                    {
                        worksheet.Cells[1, 6 + i].Value = docColumns[i]; // Starting from column 6
                    }

                    // Fill data rows
                    var rowIndex = 2;
                    foreach (var student in studentDocuments)
                    {
                        worksheet.Cells[rowIndex, 1].Value = rowIndex - 1; ;
                        worksheet.Cells[rowIndex, 2].Value = student.Student_Name;
                        worksheet.Cells[rowIndex, 3].Value = student.Admission_Number;
                        worksheet.Cells[rowIndex, 4].Value = student.Class_Name;
                        worksheet.Cells[rowIndex, 5].Value = student.Section_Name;

                        // Fill document statuses (True/False or empty if not found)
                        for (int i = 0; i < docColumns.Count; i++)
                        {
                            var docName = docColumns[i];
                            worksheet.Cells[rowIndex, 6 + i].Value = student.DocumentStatus.ContainsKey(docName)
                                ? (student.DocumentStatus[docName].IsSubmitted ? "True" : "False")
                                : "";
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
            else if (exportFormat == 2) // CSV Export
            {
                var csvFilePath = Path.Combine(directoryPath, $"{fileName}.csv");

                // Create CSV headers with both fixed and dynamic columns
                var docColumns = studentDocuments.SelectMany(s => s.DocumentStatus.Keys).Distinct().ToList();
                var headers = new List<string>
        {
            "Sr No", "Student Name", "Admission Number", "Class Name", "Section Name"
        };
                headers.AddRange(docColumns); // Add document columns

                var csvLines = new List<string> { string.Join(",", headers) };
                var rowIndex = 1;
                // Add data rows for CSV
                foreach (var student in studentDocuments)
                {
                    var row = new List<string>
            {
               rowIndex++.ToString(),
                student.Student_Name,
                student.Admission_Number,
                student.Class_Name,
                student.Section_Name
            };

                    // Fill document statuses (True/False or empty if not found)
                    foreach (var docName in docColumns)
                    {
                        row.Add(student.DocumentStatus.ContainsKey(docName)
                            ? (student.DocumentStatus[docName].IsSubmitted ? "True" : "False")
                            : "");
                    }

                    csvLines.Add(string.Join(",", row));
                }

                await File.WriteAllLinesAsync(csvFilePath, csvLines);
                return new ServiceResponse<string>(true, "CSV file generated successfully", csvFilePath, 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid export format", null, 400);
            }
        }



    }
}
