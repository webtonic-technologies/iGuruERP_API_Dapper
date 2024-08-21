using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;

namespace VisitorManagement_API.Services.Interfaces
{
    public interface ISourceService
    {
        Task<ServiceResponse<string>> AddUpdateSource(Sources source);
        Task<ServiceResponse<IEnumerable<Sources>>> GetAllSources(GetAllSourcesRequest request);
        Task<ServiceResponse<Sources>> GetSourceById(int sourceId);
        Task<ServiceResponse<bool>> UpdateSourceStatus(int sourceId);
    }
}
