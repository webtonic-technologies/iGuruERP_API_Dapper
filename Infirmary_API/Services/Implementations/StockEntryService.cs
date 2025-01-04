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

        public async Task<ServiceResponse<string>> EnterInfirmaryStockAdjustment(EnterInfirmaryStockAdjustmentRequest request)
        {
            try
            {
                // Call the repository to insert the stock adjustment
                var result = await _stockEntryRepository.EnterStockAdjustment(request);

                if (result)
                {
                    return new ServiceResponse<string>(true, "Stock adjustment entered successfully", null, 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Failed to enter stock adjustment", null, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<StockHistoryResponse>>> GetStockHistory(StockHistoryRequest request)
        {
            try
            {
                // Call the repository to get the stock history and the total count
                var historyResponse = await _stockEntryRepository.GetStockHistory(request);

                // Return the stock history with the total count
                return new ServiceResponse<List<StockHistoryResponse>>(true, "Stock history retrieved successfully", historyResponse.Data, 200, historyResponse.TotalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StockHistoryResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<GetStockInfoResponse>> GetStockInfo(GetStockInfoRequest request)
        {
            return await _stockEntryRepository.GetStockInfo(request);
        }


        public async Task<ServiceResponse<byte[]>> GetStockHistoryExport(GetStockHistoryExportRequest request)
        {
            var reportData = await _stockEntryRepository.GetStockHistoryExport(request);

            if (request.ExportType == 1)
            {
                // Excel Export (using EPPlus)
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("StockHistoryReport");
                    worksheet.Cells[1, 1].Value = "QtyID";
                    worksheet.Cells[1, 2].Value = "BatchCode";
                    worksheet.Cells[1, 3].Value = "QuantityAdjusted";
                    worksheet.Cells[1, 4].Value = "CurrentStock";
                    worksheet.Cells[1, 5].Value = "UpdatedBy";
                    worksheet.Cells[1, 6].Value = "Reason";

                    int row = 2;
                    foreach (var item in reportData)
                    {
                        worksheet.Cells[row, 1].Value = item.QtyID;
                        worksheet.Cells[row, 2].Value = item.BatchCode;
                        worksheet.Cells[row, 3].Value = item.QuantityAdjusted;
                        worksheet.Cells[row, 4].Value = item.CurrentStock;
                        worksheet.Cells[row, 5].Value = item.UpdatedBy;
                        worksheet.Cells[row, 6].Value = item.Reason;
                        row++;
                    }

                    var fileBytes = package.GetAsByteArray();
                    return new ServiceResponse<byte[]>(true, "Excel export successful", fileBytes, 200);
                }
            }
            else if (request.ExportType == 2)
            {
                // CSV Export (using CsvHelper)
                var csvBuilder = new StringBuilder();
                using (var writer = new StringWriter(csvBuilder))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<GetStockHistoryExportResponse>();
                    csv.NextRecord();

                    foreach (var item in reportData)
                    {
                        csv.WriteRecord(item);
                        csv.NextRecord();
                    }
                }

                var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
                return new ServiceResponse<byte[]>(true, "CSV export successful", fileBytes, 200);
            }
            else
            {
                return new ServiceResponse<byte[]>(false, "Invalid export type", null, 400);
            }
        }

    }
}
