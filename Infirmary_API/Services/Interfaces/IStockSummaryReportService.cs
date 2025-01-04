using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.DTOs.Responses;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IStockSummaryReportService
    {
        Task<ServiceResponse<List<GetStockSummaryReportResponse>>> GetStockSummaryReport(GetStockSummaryReportRequest request);
        Task<ServiceResponse<byte[]>> GetStockSummaryReportExport(GetStockSummaryReportExportRequest request);

    }
}
