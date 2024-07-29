using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using System.Collections.Generic;
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
    }
}
