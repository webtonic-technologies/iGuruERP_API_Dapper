using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Interfaces
{
    public interface IStockEntryRepository
    {
        Task<ServiceResponse<string>> AddUpdateStockEntry(AddUpdateStockEntryRequest request);
        Task<ServiceResponse<List<StockEntryResponse>>> GetAllStockEntries(GetAllStockEntriesRequest request);
        Task<ServiceResponse<StockEntry>> GetStockEntryById(int id);
        Task<ServiceResponse<bool>> DeleteStockEntry(int id);
        Task<List<GetStockEntriesExportResponse>> GetStockEntriesData(int instituteId, string startDate, string endDate, string searchTerm);

    }
}
