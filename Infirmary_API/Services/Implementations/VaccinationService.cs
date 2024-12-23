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
    public class VaccinationService : IVaccinationService
    {
        private readonly IVaccinationRepository _vaccinationRepository;

        public VaccinationService(IVaccinationRepository vaccinationRepository)
        {
            _vaccinationRepository = vaccinationRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateVaccination(AddUpdateVaccinationRequest request)
        {
            return await _vaccinationRepository.AddUpdateVaccination(request);
        }

        public async Task<ServiceResponse<List<VaccinationResponse>>> GetAllVaccinations(GetAllVaccinationsRequest request)
        {
            return await _vaccinationRepository.GetAllVaccinations(request);
        }

        public async Task<ServiceResponse<Vaccination>> GetVaccinationById(int id)
        {
            return await _vaccinationRepository.GetVaccinationById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteVaccination(int id)
        {
            return await _vaccinationRepository.DeleteVaccination(id);
        }

        public async Task<ServiceResponse<byte[]>> ExportVaccinationData(GetVaccinationsExportRequest request)
        {
            var data = await _vaccinationRepository.GetVaccinationData(request.InstituteID);

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

        private byte[] GenerateExcel(List<GetVaccinationsExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Vaccinations");
                worksheet.Cells["A1"].Value = "Vaccination Name";
                worksheet.Cells["B1"].Value = "Description";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.VaccinationName;
                    worksheet.Cells[row, 2].Value = item.Description;
                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }

        private byte[] GenerateCsv(List<GetVaccinationsExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<GetVaccinationsExportResponse>();
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
