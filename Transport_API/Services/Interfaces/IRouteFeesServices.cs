using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;

namespace Transport_API.Services.Interfaces
{
    public interface IRouteFeesServices
    {
        Task<ServiceResponse<string>> AddUpdateRouteFeeStructure(RouteFeeStructure request);
        Task<ServiceResponse<RouteFeeStructure>> GetRouteFeeStructureById(int routeFeeStructureId);
    }
}
