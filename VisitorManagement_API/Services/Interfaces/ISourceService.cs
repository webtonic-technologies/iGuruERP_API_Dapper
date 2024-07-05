using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;

namespace VisitorManagement_API.Services.Interfaces
{
    public interface ISourceService
    {
        Task<ServiceResponse<string>> AddUpdateSource(Source source);
        Task<ServiceResponse<IEnumerable<Source>>> GetAllSources(GetAllSourcesRequest request);
        Task<ServiceResponse<Source>> GetSourceById(int sourceId);
        Task<ServiceResponse<bool>> UpdateSourceStatus(int sourceId);
    }
}
