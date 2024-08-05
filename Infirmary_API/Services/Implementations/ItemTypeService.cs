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
    public class ItemTypeService : IItemTypeService
    {
        private readonly IItemTypeRepository _itemTypeRepository;

        public ItemTypeService(IItemTypeRepository itemTypeRepository)
        {
            _itemTypeRepository = itemTypeRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateItemType(AddUpdateItemTypeRequest request)
        {
            return await _itemTypeRepository.AddUpdateItemType(request);
        }

        public async Task<ServiceResponse<List<ItemTypeResponse>>> GetAllItemTypes(GetAllItemTypesRequest request)
        {
            return await _itemTypeRepository.GetAllItemTypes(request);
        }

        public async Task<ServiceResponse<ItemType>> GetItemTypeById(int id)
        {
            return await _itemTypeRepository.GetItemTypeById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteItemType(int id)
        {
            return await _itemTypeRepository.DeleteItemType(id);
        }
    }
}
