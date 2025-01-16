using Communication_API.DTOs.Requests.DigitalDiary;
using Communication_API.DTOs.Responses;
using Communication_API.DTOs.Responses.DigitalDiary;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DigitalDiary;
using Communication_API.Repository.Interfaces.DigitalDiary;
using Communication_API.Services.Interfaces.DigitalDiary;
using CsvHelper;
using OfficeOpenXml;
using System.Globalization;
using System.Text;

namespace Communication_API.Services.Implementations.DigitalDiary
{
    public class DigitalDiaryService : IDigitalDiaryService
    {
        private readonly IDigitalDiaryRepository _digitalDiaryRepository;

        public DigitalDiaryService(IDigitalDiaryRepository digitalDiaryRepository)
        {
            _digitalDiaryRepository = digitalDiaryRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateDiary(AddUpdateDiaryRequest request)
        {
            return await _digitalDiaryRepository.AddUpdateDiary(request);
        }

        public async Task<ServiceResponse<List<DiaryResponse>>> GetAllDiary(GetAllDiaryRequest request)
        {
            return await _digitalDiaryRepository.GetAllDiary(request);
        }

        public async Task<ServiceResponse<string>> DeleteDiary(int DiaryID)
        {
            return await _digitalDiaryRepository.DeleteDiary(DiaryID);
        }

        public async Task<ServiceResponse<byte[]>> GetAllDiaryExport(GetAllDiaryExportRequest request)
        {
            try
            {
                var diaryRecords = await _digitalDiaryRepository.GetAllDiaryExport(request);

                byte[] exportData = null;

                if (request.ExportType == 1) // Excel export
                {
                    exportData = GenerateExcel(diaryRecords);
                }
                else if (request.ExportType == 2) // CSV export
                {
                    exportData = GenerateCsv(diaryRecords);
                }

                return new ServiceResponse<byte[]>(
                    true,
                    "Export completed successfully",
                    exportData,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        private byte[] GenerateExcel(IEnumerable<GetAllDiaryExportResponse> diaryRecords)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("DigitalDiary");

                worksheet.Cells["A1"].Value = "Student Name";
                worksheet.Cells["B1"].Value = "Admission Number";
                worksheet.Cells["C1"].Value = "Class Section";
                worksheet.Cells["D1"].Value = "Subject";
                worksheet.Cells["E1"].Value = "Diary Remarks";
                worksheet.Cells["F1"].Value = "Share On";
                worksheet.Cells["G1"].Value = "Given By";

                int row = 2;
                foreach (var record in diaryRecords)
                {
                    worksheet.Cells[row, 1].Value = record.StudentName;
                    worksheet.Cells[row, 2].Value = record.AdmissionNumber;
                    worksheet.Cells[row, 3].Value = record.ClassSection;
                    worksheet.Cells[row, 4].Value = record.Subject;
                    worksheet.Cells[row, 5].Value = record.DiaryRemarks;
                    worksheet.Cells[row, 6].Value = record.ShareOn;
                    worksheet.Cells[row, 7].Value = record.GivenBy;
                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }

        private byte[] GenerateCsv(IEnumerable<GetAllDiaryExportResponse> diaryRecords)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                // Write header
                csv.WriteHeader<GetAllDiaryExportResponse>();
                csv.NextRecord();

                // Write data
                foreach (var record in diaryRecords)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
