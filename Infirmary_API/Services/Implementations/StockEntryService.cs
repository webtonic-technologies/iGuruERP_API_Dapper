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
    public class StockEntryService : IStockEntryService
    {
        private readonly IStockEntryRepository _stockEntryRepository;

        public StockEntryService(IStockEntryRepository stockEntryRepository)
        {
            _stockEntryRepository = stockEntryRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateStockEntry(AddUpdateStockEntryRequest request)
        {
            return await _stockEntryRepository.AddUpdateStockEntry(request);
        }

        public async Task<ServiceResponse<List<StockEntryResponse>>> GetAllStockEntries(GetAllStockEntriesRequest request)
        {
            return await _stockEntryRepository.GetAllStockEntries(request);
        }

        public async Task<ServiceResponse<StockEntry>> GetStockEntryById(int id)
        {
            return await _stockEntryRepository.GetStockEntryById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteStockEntry(int id)
        {
            return await _stockEntryRepository.DeleteStockEntry(id);
        }

        public async Task<ServiceResponse<byte[]>> ExportStockEntriesData(GetStockEntriesExportRequest request)
        {
            var data = await _stockEntryRepository.GetStockEntriesData(request.InstituteID, request.StartDate, request.EndDate, request.SearchTerm);

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

        private byte[] GenerateExcel(List<GetStockEntriesExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Stock Entries");
                worksheet.Cells["A1"].Value = "Item Type";
                worksheet.Cells["B1"].Value = "Medicine Name";
                worksheet.Cells["C1"].Value = "Company";
                worksheet.Cells["D1"].Value = "Batch Code";
                worksheet.Cells["E1"].Value = "Diagnosis";
                worksheet.Cells["F1"].Value = "Quantity";
                worksheet.Cells["G1"].Value = "Price per Quantity";
                worksheet.Cells["H1"].Value = "Expiry Date";
                worksheet.Cells["I1"].Value = "Entry Date";
                worksheet.Cells["J1"].Value = "Dosage Details";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.ItemTypeName;
                    worksheet.Cells[row, 2].Value = item.MedicineName;
                    worksheet.Cells[row, 3].Value = item.Company;
                    worksheet.Cells[row, 4].Value = item.BatchCode;
                    worksheet.Cells[row, 5].Value = item.Diagnosis;
                    worksheet.Cells[row, 6].Value = item.Quantity;
                    worksheet.Cells[row, 7].Value = item.PricePerQuantity;
                    worksheet.Cells[row, 8].Value = item.ExpiryDate;
                    worksheet.Cells[row, 9].Value = item.EntryDate;
                    worksheet.Cells[row, 10].Value = item.DosageDetails;
                    row++;
                }

                package.Save();
                return memoryStream.ToArray();
            }
        }

        private byte[] GenerateCsv(List<GetStockEntriesExportResponse> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<GetStockEntriesExportResponse>();
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
