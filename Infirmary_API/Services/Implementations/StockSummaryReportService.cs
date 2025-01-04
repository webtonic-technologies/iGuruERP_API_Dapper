using CsvHelper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using OfficeOpenXml;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class StockSummaryReportService : IStockSummaryReportService
    {
        private readonly IStockSummaryReportRepository _stockSummaryReportRepository;

        public StockSummaryReportService(IStockSummaryReportRepository stockSummaryReportRepository)
        {
            _stockSummaryReportRepository = stockSummaryReportRepository;
        }

        public async Task<ServiceResponse<List<GetStockSummaryReportResponse>>> GetStockSummaryReport(GetStockSummaryReportRequest request)
        {
            return await _stockSummaryReportRepository.GetStockSummaryReport(request);
        }
        public async Task<ServiceResponse<byte[]>> GetStockSummaryReportExport(GetStockSummaryReportExportRequest request)
        {
            var reportData = await _stockSummaryReportRepository.GetStockSummaryReportExport(request);

            if (request.ExportType == 1)
            {
                // Excel Export (using EPPlus)
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("StockSummaryReport");
                    worksheet.Cells[1, 1].Value = "Item Type";
                    worksheet.Cells[1, 2].Value = "Item Name";
                    worksheet.Cells[1, 3].Value = "Company";
                    worksheet.Cells[1, 4].Value = "Batch Code";
                    worksheet.Cells[1, 5].Value = "Opening Stock";
                    worksheet.Cells[1, 6].Value = "In Quantity";
                    worksheet.Cells[1, 7].Value = "Out Quantity";
                    worksheet.Cells[1, 8].Value = "Closing Stock";

                    int row = 2;
                    foreach (var item in reportData)
                    {
                        worksheet.Cells[row, 1].Value = item.ItemType;
                        worksheet.Cells[row, 2].Value = item.ItemName;
                        worksheet.Cells[row, 3].Value = item.Company;
                        worksheet.Cells[row, 4].Value = item.BatchCode;
                        worksheet.Cells[row, 5].Value = item.OpeningStock;
                        worksheet.Cells[row, 6].Value = item.InQuantity;
                        worksheet.Cells[row, 7].Value = item.OutQuantity;
                        worksheet.Cells[row, 8].Value = item.ClosingStock;
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
                    csv.WriteHeader<GetStockSummaryReportExportResponse>();
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
