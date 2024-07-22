using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Services.Implementations
{
    public class CatalogueService : ICatalogueService
    {
        private readonly ICatalogueRepository _catalogueRepository;

        public CatalogueService(ICatalogueRepository catalogueRepository)
        {
            _catalogueRepository = catalogueRepository;
        }

        public async Task<ServiceResponse<List<CatalogueResponse>>> GetAllCatalogues(GetAllCataloguesRequest request)
        {
            return await _catalogueRepository.GetAllCatalogues(request);
        }

        public async Task<ServiceResponse<Catalogue>> GetCatalogueById(int catalogueId)
        {
            return await _catalogueRepository.GetCatalogueById(catalogueId);
        }

        public async Task<ServiceResponse<string>> AddUpdateCatalogue(Catalogue request)
        {
            return await _catalogueRepository.AddUpdateCatalogue(request);
        }

        public async Task<ServiceResponse<bool>> DeleteCatalogue(int catalogueId)
        {
            return await _catalogueRepository.DeleteCatalogue(catalogueId);
        }
    }
}
