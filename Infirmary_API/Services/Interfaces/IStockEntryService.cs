using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IStockEntryService
    {
        Task<ServiceResponse<string>> AddUpdateStockEntry(AddUpdateStockEntryRequest request);
        Task<ServiceResponse<List<StockEntryResponse>>> GetAllStockEntries(GetAllStockEntriesRequest request);
        Task<ServiceResponse<StockEntry>> GetStockEntryById(int id);
        Task<ServiceResponse<bool>> DeleteStockEntry(int id);
        Task<ServiceResponse<byte[]>> ExportStockEntriesData(GetStockEntriesExportRequest request);
        Task<ServiceResponse<string>> EnterInfirmaryStockAdjustment(EnterInfirmaryStockAdjustmentRequest request);
        Task<ServiceResponse<List<StockHistoryResponse>>> GetStockHistory(StockHistoryRequest request);
        Task<ServiceResponse<GetStockInfoResponse>> GetStockInfo(GetStockInfoRequest request);
        Task<ServiceResponse<byte[]>> GetStockHistoryExport(GetStockHistoryExportRequest request);

    }
}
