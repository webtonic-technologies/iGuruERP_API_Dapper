using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class ItemTypeFetchService : IItemTypeFetchService
    {
        private readonly IItemTypeFetchRepository _itemTypeFetchRepository;

        public ItemTypeFetchService(IItemTypeFetchRepository itemTypeFetchRepository)
        {
            _itemTypeFetchRepository = itemTypeFetchRepository;
        }

        public async Task<ServiceResponse<List<ItemTypeFetchResponse>>> GetAllItemTypesFetch(GetAllItemTypesFetchRequest request)
        {
            return await _itemTypeFetchRepository.GetAllItemTypesFetch(request);
        }
    }
}
