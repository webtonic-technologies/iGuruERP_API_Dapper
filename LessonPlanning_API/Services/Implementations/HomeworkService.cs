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
    public class HomeworkService : IHomeworkService
    {
        private readonly IHomeworkRepository _homeworkRepository;

        public HomeworkService(IHomeworkRepository homeworkRepository)
        {
            _homeworkRepository = homeworkRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateHomework(HomeworkRequest request)
        {
            return await _homeworkRepository.AddUpdateHomework(request);
        }

        public async Task<ServiceResponse<List<GetAllHomeworkResponse>>> GetAllHomework(GetAllHomeworkRequest request)
        {
            return await _homeworkRepository.GetAllHomework(request);
        }

        public async Task<ServiceResponse<Homework>> GetHomeworkById(int id)
        {
            return await _homeworkRepository.GetHomeworkById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteHomework(int id)
        {
            return await _homeworkRepository.DeleteHomework(id);
        }
        public async Task<ServiceResponse<GetHomeworkHistoryResponse>> GetHomeworkHistory(GetHomeworkHistoryRequest request)
        {
            return await _homeworkRepository.GetHomeworkHistory(request);
        }

        public async Task<ServiceResponse<byte[]>> GetAllHomeworkExport(GetAllHomeworkExportRequest request)
        {
            try
            {
                var homeworkData = await _homeworkRepository.GetAllHomeworkForExport(request);

                if (homeworkData == null || !homeworkData.Any())
                {
                    return new ServiceResponse<byte[]>(
                        null,             // No data to export
                        false,            // Failure flag
                        "No data found for the specified criteria.", // Error message
                        404               // HTTP status code (404 for not found)
                    );
                }

                byte[] exportData = null;

                // Generate the export data based on the ExportType
                if (request.ExportType == 1) // Excel export
                {
                    exportData = GenerateExcel(homeworkData); // Generate Excel byte array
                }
                else if (request.ExportType == 2) // CSV export
                {
                    exportData = GenerateCsv(homeworkData); // Generate CSV byte array
                }

                // Ensure export data is valid (not null)
                if (exportData == null)
                {
                    return new ServiceResponse<byte[]>(
                        null,             // No file content due to failure
                        false,            // Failure flag
                        "Failed to generate the export file.", // Error message
                        500               // HTTP status code (500 for server error)
                    );
                }

                // Return the file content as byte array, indicating success
                return new ServiceResponse<byte[]>(
                    exportData,       // First argument is the byte[] file content
                    true,             // Success flag
                    "Export completed successfully", // Success message
                    200               // HTTP status code (200 for success)
                );
            }
            catch (Exception ex)
            {
                // Return failure response in case of an exception
                return new ServiceResponse<byte[]>(
                    null,             // No file content due to failure
                    false,            // Failure flag
                    ex.Message,       // Error message
                    500               // HTTP status code (500 for server error)
                );
            }
        }


        //private byte[] GenerateExcel(IEnumerable<GetAllHomeworkExportResponse> homeworkData)
        //{
        //    if (homeworkData == null || !homeworkData.Any())
        //    {
        //        return null; // If no data is available, return null
        //    }

        //    using (var memoryStream = new MemoryStream())
        //    using (var package = new ExcelPackage(memoryStream))
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("HomeworkData");
        //        worksheet.Cells["A1"].Value = "Homework Name";
        //        worksheet.Cells["B1"].Value = "Subject Name";
        //        worksheet.Cells["C1"].Value = "Homework Type";
        //        worksheet.Cells["D1"].Value = "Notes";
        //        worksheet.Cells["E1"].Value = "Created By";
        //        worksheet.Cells["F1"].Value = "Created On";
        //        worksheet.Cells["G1"].Value = "Class Name";
        //        worksheet.Cells["H1"].Value = "Section Name";

        //        int row = 2;

        //        foreach (var item in homeworkData)
        //        {
        //            // Check if ClassSections is null or empty
        //            if (item.ClassSections != null && item.ClassSections.Any())
        //            {
        //                foreach (var classSection in item.ClassSections)
        //                {
        //                    worksheet.Cells[row, 1].Value = item.HomeworkName;
        //                    worksheet.Cells[row, 2].Value = item.SubjectName;
        //                    worksheet.Cells[row, 3].Value = item.HomeworkType;
        //                    worksheet.Cells[row, 4].Value = item.Notes;
        //                    worksheet.Cells[row, 5].Value = item.CreatedBy;
        //                    worksheet.Cells[row, 6].Value = item.CreatedOn;
        //                    worksheet.Cells[row, 7].Value = classSection.ClassName;
        //                    worksheet.Cells[row, 8].Value = classSection.SectionName;
        //                    row++;
        //                }
        //            }
        //            else
        //            {
        //                // Handle case where ClassSections is null or empty
        //                worksheet.Cells[row, 1].Value = item.HomeworkName;
        //                worksheet.Cells[row, 2].Value = item.SubjectName;
        //                worksheet.Cells[row, 3].Value = item.HomeworkType;
        //                worksheet.Cells[row, 4].Value = item.Notes;
        //                worksheet.Cells[row, 5].Value = item.CreatedBy;
        //                worksheet.Cells[row, 6].Value = item.CreatedOn;
        //                worksheet.Cells[row, 7].Value = "N/A";  // Placeholder for missing ClassName
        //                worksheet.Cells[row, 8].Value = "N/A";  // Placeholder for missing SectionName
        //                row++;
        //            }
        //        }

        //        // Save to memory stream and return byte array
        //        package.Save();
        //        return memoryStream.ToArray();
        //    }
        //}


        private byte[] GenerateExcel(IEnumerable<GetAllHomeworkExportResponse> homeworkData)
        {
            if (homeworkData == null || !homeworkData.Any())
            {
                return null; // If no data is available, return null
            }

            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("HomeworkData");
                worksheet.Cells["A1"].Value = "Homework Name";
                worksheet.Cells["B1"].Value = "Subject Name";
                worksheet.Cells["C1"].Value = "Homework Type";
                worksheet.Cells["D1"].Value = "Notes";
                worksheet.Cells["E1"].Value = "Created By";
                worksheet.Cells["F1"].Value = "Created On";
                worksheet.Cells["G1"].Value = "Class Sections"; // Changed to Class Sections column

                int row = 2;

                foreach (var item in homeworkData)
                {
                    // Combine all class sections into a comma-separated string
                    string classSections = string.Join(", ", item.ClassSections.Select(cs => $"{cs.ClassName} - {cs.SectionName}"));

                    // If there are no class sections, use "N/A"
                    if (string.IsNullOrEmpty(classSections))
                    {
                        classSections = "N/A";
                    }

                    worksheet.Cells[row, 1].Value = item.HomeworkName;
                    worksheet.Cells[row, 2].Value = item.SubjectName;
                    worksheet.Cells[row, 3].Value = item.HomeworkType;
                    worksheet.Cells[row, 4].Value = item.Notes;
                    worksheet.Cells[row, 5].Value = item.CreatedBy;
                    worksheet.Cells[row, 6].Value = item.CreatedOn;
                    worksheet.Cells[row, 7].Value = classSections; // Insert combined class section string into column G

                    row++;
                }

                // Save to memory stream and return byte array
                package.Save();
                return memoryStream.ToArray();
            }
        }


        private byte[] GenerateCsv(IEnumerable<GetAllHomeworkExportResponse> homeworkData)
        {
            if (homeworkData == null || !homeworkData.Any())
            {
                return null; // If no data is available, return null
            }

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                // Write header
                csv.WriteHeader<GetAllHomeworkExportResponse>();
                csv.NextRecord();

                // Write data
                foreach (var item in homeworkData)
                {
                    foreach (var classSection in item.ClassSections)
                    {
                        csv.WriteField(item.HomeworkName);
                        csv.WriteField(item.SubjectName);
                        csv.WriteField(item.HomeworkType);
                        csv.WriteField(item.Notes);
                        csv.WriteField(item.CreatedBy);
                        csv.WriteField(item.CreatedOn);
                        csv.WriteField(classSection.ClassName);
                        csv.WriteField(classSection.SectionName);
                        csv.NextRecord();
                    }
                }

                // Flush the data to memory stream and return byte array
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }


    }
}
