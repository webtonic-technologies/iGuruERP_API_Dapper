using CsvHelper;
using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using OfficeOpenXml;
using System.Globalization;
using System.Text;

namespace Lesson_API.Services.Implementations
{
    public class AssignmentReportService : IAssignmentReportService
    {
        private readonly IAssignmentReportRepository _assignmentReportRepository;

        public AssignmentReportService(IAssignmentReportRepository assignmentReportRepository)
        {
            _assignmentReportRepository = assignmentReportRepository;
        }

        public async Task<ServiceResponse<List<GetAssignmentsReportsResponse>>> GetAssignmentsReports(GetAssignmentsReportsRequest request)
        {
            try
            {
                var data = await _assignmentReportRepository.GetAssignmentsReports(request);

                return new ServiceResponse<List<GetAssignmentsReportsResponse>>(data, true, "Assignments retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetAssignmentsReportsResponse>>(null, false, $"Error occurred: {ex.Message}", 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> GetAssignmentsReportExport(GetAssignmentsReportsExportRequest request)
        {
            try
            {
                // Fetch the assignments from the repository
                var assignments = await _assignmentReportRepository.GetAssignmentsReportsExport(request);

                byte[] exportData = null;

                // Check for export type (Excel or CSV)
                if (request.ExportType == 1) // Export to Excel
                {
                    exportData = GenerateExcel(assignments);  // Assuming GenerateExcel is implemented
                }
                else if (request.ExportType == 2) // Export to CSV
                {
                    exportData = GenerateCsv(assignments);    // Assuming GenerateCsv is implemented
                }

                // Return the ServiceResponse with the byte array (exported file data)
                return new ServiceResponse<byte[]>(
                    exportData,  // This is the byte[] that will be returned
                    true,        // Success flag
                    "Export completed successfully", // Message
                    200          // HTTP status code
                );
            }
            catch (Exception ex)
            {
                // Handle any errors during the export process
                return new ServiceResponse<byte[]>(
                    null,         // No data on error
                    false,        // Failure flag
                    ex.Message,   // Error message
                    500           // Internal server error
                );
            }
        }


        private byte[] GenerateExcel(IEnumerable<GetAssignmentsReportsExportResponse> assignments)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("AssignmentsReport");

                // Header row
                worksheet.Cells["A1"].Value = "Assignment Name";
                worksheet.Cells["B1"].Value = "Subject Name";
                worksheet.Cells["C1"].Value = "Assignment Type";
                worksheet.Cells["D1"].Value = "Description";
                worksheet.Cells["E1"].Value = "Reference";
                worksheet.Cells["F1"].Value = "Start Date";
                worksheet.Cells["G1"].Value = "Submission Date";
                worksheet.Cells["H1"].Value = "Is Active";
                worksheet.Cells["I1"].Value = "Created By";
                worksheet.Cells["J1"].Value = "Created On";
                worksheet.Cells["K1"].Value = "Class Sections";
                worksheet.Cells["L1"].Value = "Students";

                int row = 2;
                foreach (var assignment in assignments)
                {
                    worksheet.Cells[row, 1].Value = assignment.AssignmentName;
                    worksheet.Cells[row, 2].Value = assignment.SubjectName;
                    worksheet.Cells[row, 3].Value = assignment.AssignmentType;
                    worksheet.Cells[row, 4].Value = assignment.Description;
                    worksheet.Cells[row, 5].Value = assignment.Reference;
                    worksheet.Cells[row, 6].Value = assignment.StartDate;
                    worksheet.Cells[row, 7].Value = assignment.SubmissionDate;
                    worksheet.Cells[row, 8].Value = assignment.IsActive ? "Yes" : "No";
                    worksheet.Cells[row, 9].Value = assignment.CreatedBy;
                    worksheet.Cells[row, 10].Value = assignment.CreatedOn;
                    worksheet.Cells[row, 11].Value = assignment.ClassSections;
                    worksheet.Cells[row, 12].Value = assignment.Students;
                    ////worksheet.Cells[row, 11].Value = string.Join(", ", assignment.ClassSections.Select(cs => $"{cs.ClassName} {cs.SectionName}"));
                    //worksheet.Cells[row, 11].Value = string.Join(", ", assignment.ClassSections?.Select(cs => $"{cs.ClassName} {cs.SectionName}") ?? new List<string>());
                    ////worksheet.Cells[row, 12].Value = string.Join(", ", assignment.Students.Select(s => s.StudentName));
                    //worksheet.Cells[row, 12].Value = string.Join(", ", assignment.Students?.Select(s => s.StudentName) ?? new List<string>());


                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }

        private byte[] GenerateCsv(IEnumerable<GetAssignmentsReportsExportResponse> assignments)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<GetAssignmentsReportsExportResponse>();
                csv.NextRecord();

                foreach (var assignment in assignments)
                {
                    csv.WriteRecord(assignment);
                    csv.NextRecord();
                }

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
