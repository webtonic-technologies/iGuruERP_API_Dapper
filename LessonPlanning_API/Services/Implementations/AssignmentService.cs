using CsvHelper;
using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Lesson_API.Services.Implementations
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentService(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateAssignment(AssignmentRequest request)
        {
            return await _assignmentRepository.AddUpdateAssignment(request);
        }

        public async Task<ServiceResponse<List<GetAllAssignmentsResponse>>> GetAllAssignments(GetAllAssignmentsRequest request)
        {
            return await _assignmentRepository.GetAllAssignments(request);
        }

        public async Task<ServiceResponse<Assignment>> GetAssignmentById(int id)
        {
            return await _assignmentRepository.GetAssignmentById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteAssignment(int id)
        {
            return await _assignmentRepository.DeleteAssignment(id);
        }

        public async Task<ServiceResponse<byte[]>> DownloadDocument(int documentId)
        {
            return await _assignmentRepository.DownloadDocument(documentId);
        }


        public async Task<ServiceResponse<byte[]>> GetAssignmentsExport(GetAssignmentsExportRequest request)
        {
            try
            {
                var assignments = await _assignmentRepository.GetAssignmentsExport(request);
                byte[] exportData = null;

                if (request.ExportType == 1) // Excel export
                {
                    exportData = GenerateExcel(assignments);
                }
                else if (request.ExportType == 2) // CSV export
                {
                    exportData = GenerateCsv(assignments);
                }

                return new ServiceResponse<byte[]>(
                    exportData,
                    true,
                    "Export completed successfully",
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(
                    null,
                    false,
                    ex.Message,
                    500
                );
            }
        }

        private byte[] GenerateExcel(IEnumerable<GetAssignmentsExportResponse> assignments)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Assignments");
                worksheet.Cells["A1"].Value = "Assignment Name";
                worksheet.Cells["B1"].Value = "Subject";
                worksheet.Cells["C1"].Value = "Assignment Type";
                worksheet.Cells["D1"].Value = "Description";
                worksheet.Cells["E1"].Value = "Reference";
                worksheet.Cells["F1"].Value = "Start Date";
                worksheet.Cells["G1"].Value = "Submission Date";
                worksheet.Cells["H1"].Value = "Created By";
                worksheet.Cells["I1"].Value = "Created On";
                worksheet.Cells["J1"].Value = "Class & Section/Students";

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
                    worksheet.Cells[row, 8].Value = assignment.CreatedBy;
                    worksheet.Cells[row, 9].Value = assignment.CreatedOn;
                    worksheet.Cells[row, 10].Value = assignment.StudentsOrClassSection;  // Show either class & section or students
                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }

        private byte[] GenerateCsv(IEnumerable<GetAssignmentsExportResponse> assignments)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<GetAssignmentsExportResponse>();
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

        public async Task<ServiceResponse<List<GetTypeWiseResponse>>> GetTypeWise()
        {
            try
            {
                var typeWiseData = await _assignmentRepository.GetTypeWise();

                if (typeWiseData == null || typeWiseData.Count == 0)
                {
                    return new ServiceResponse<List<GetTypeWiseResponse>>(null, false, "No TypeWise data found.", 404);
                }

                return new ServiceResponse<List<GetTypeWiseResponse>>(typeWiseData, true, "Data retrieved successfully.", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetTypeWiseResponse>>(null, false, ex.Message, 500);
            }
        }

    }
}
