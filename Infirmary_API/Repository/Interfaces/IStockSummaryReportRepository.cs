using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Responses;
using System.Threading.Tasks;
using Infirmary_API.DTOs.ServiceResponse;

namespace Infirmary_API.Repository.Interfaces
{
    public interface IStockSummaryReportRepository
    {
        Task<ServiceResponse<List<GetStockSummaryReportResponse>>> GetStockSummaryReport(GetStockSummaryReportRequest request);
        Task<List<GetStockSummaryReportExportResponse>> GetStockSummaryReportExport(GetStockSummaryReportExportRequest request);

    }
}
