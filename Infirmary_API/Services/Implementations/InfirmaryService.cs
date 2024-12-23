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
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class InfirmaryService : IInfirmaryService
    {
        private readonly IInfirmaryRepository _infirmaryRepository;

        public InfirmaryService(IInfirmaryRepository infirmaryRepository)
        {
            _infirmaryRepository = infirmaryRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateInfirmary(AddUpdateInfirmaryRequest request)
        {
            return await _infirmaryRepository.AddUpdateInfirmary(request);
        }

        public async Task<ServiceResponse<List<InfirmaryResponse>>> GetAllInfirmary(GetAllInfirmaryRequest request)
        {
            return await _infirmaryRepository.GetAllInfirmary(request);
        }

        public async Task<ServiceResponse<Infirmary>> GetInfirmaryById(int id)
        {
            return await _infirmaryRepository.GetInfirmaryById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteInfirmary(int id)
        {
            return await _infirmaryRepository.DeleteInfirmary(id);
        }

        public async Task<ServiceResponse<byte[]>> ExportInfirmaryData(GetInfirmaryExportRequest request)
        {
            var data = await _infirmaryRepository.GetInfirmaryData(request.InstituteID);

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

        private byte[] GenerateExcel(List<GetInfirmaryExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Infirmary Data");
                worksheet.Cells["A1"].Value = "Infirmary Name";
                worksheet.Cells["B1"].Value = "Infirmary Incharge";
                worksheet.Cells["C1"].Value = "No Of Beds";
                worksheet.Cells["D1"].Value = "Description";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.InfirmaryName;
                    worksheet.Cells[row, 2].Value = item.InfirmaryIncharge;
                    worksheet.Cells[row, 3].Value = item.NoOfBeds;
                    worksheet.Cells[row, 4].Value = item.Description;
                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }

        private byte[] GenerateCsv(List<GetInfirmaryExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<GetInfirmaryExportResponse>();
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
