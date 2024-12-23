using CsvHelper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class StudentVaccinationService : IStudentVaccinationService
    {
        private readonly IStudentVaccinationRepository _studentVaccinationRepository;

        public StudentVaccinationService(IStudentVaccinationRepository studentVaccinationRepository)
        {
            _studentVaccinationRepository = studentVaccinationRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateStudentVaccination(AddUpdateStudentVaccinationRequest request)
        {
            return await _studentVaccinationRepository.AddUpdateStudentVaccination(request);
        }

        public async Task<ServiceResponse<List<StudentVaccinationResponse>>> GetAllStudentVaccinations(GetAllStudentVaccinationsRequest request)
        {
            return await _studentVaccinationRepository.GetAllStudentVaccinations(request);
        }

        public async Task<ServiceResponse<List<StudentVaccinationResponse>>> GetStudentVaccinationById(int id)
        {
            return await _studentVaccinationRepository.GetStudentVaccinationById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteStudentVaccination(int id)
        {
            return await _studentVaccinationRepository.DeleteStudentVaccination(id);
        }

        public async Task<ServiceResponse<byte[]>> ExportStudentVaccinationsData(GetStudentVaccinationsExportRequest request)
        {
            var data = await _studentVaccinationRepository.GetStudentVaccinationsData(request.InstituteID, request.ClassID, request.SectionID, request.VaccinationID);

            if (data == null || data.Count == 0)
            {
                return new ServiceResponse<byte[]>(false, "No data found", null, 404);
            }

            byte[] exportData = null;

            if (request.ExportType == 1) // Excel export
            {
                exportData = GenerateExcel(data);
            }
            else if (request.ExportType == 2) // CSV export
            {
                exportData = GenerateCsv(data);
            }
            else
            {
                return new ServiceResponse<byte[]>(false, "Invalid ExportType", null, 400);
            }

            return new ServiceResponse<byte[]>(true, "Export completed successfully", exportData, 200);
        }

        private byte[] GenerateExcel(List<GetStudentVaccinationsExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Student Vaccinations");
                worksheet.Cells["A1"].Value = "Class Name";
                worksheet.Cells["B1"].Value = "Section Name";
                worksheet.Cells["C1"].Value = "Student Name";
                worksheet.Cells["D1"].Value = "Date of Vaccination";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.ClassName;
                    worksheet.Cells[row, 2].Value = item.SectionName;
                    worksheet.Cells[row, 3].Value = item.StudentName;
                    worksheet.Cells[row, 4].Value = item.DateOfVaccination;
                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }

        private byte[] GenerateCsv(List<GetStudentVaccinationsExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<GetStudentVaccinationsExportResponse>();
                csv.NextRecord();

                foreach (var item in data)
                {
                    csv.WriteRecord(item);
                    csv.NextRecord();
                }

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
