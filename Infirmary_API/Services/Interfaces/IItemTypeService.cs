﻿using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IItemTypeService
    {
        Task<ServiceResponse<string>> AddUpdateItemType(AddUpdateItemTypeRequest request);
        Task<ServiceResponse<List<ItemTypeResponse>>> GetAllItemTypes(GetAllItemTypesRequest request);
        Task<ServiceResponse<ItemType>> GetItemTypeById(int id);
        Task<ServiceResponse<bool>> DeleteItemType(int id);
        Task<ServiceResponse<byte[]>> ExportItemTypesData(GetItemTypesExportRequest request);

    }
}
