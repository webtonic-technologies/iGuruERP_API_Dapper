using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IItemTypeFetchService
    {
        Task<ServiceResponse<List<ItemTypeFetchResponse>>> GetAllItemTypesFetch(GetAllItemTypesFetchRequest request);
    }
}
