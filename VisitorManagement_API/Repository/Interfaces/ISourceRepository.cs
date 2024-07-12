using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.Models;  // Add this line

namespace VisitorManagement_API.Repository.Interfaces
{
    public interface ISourceRepository
    {
        Task<ServiceResponse<string>> AddUpdateSource(Sources source);
        Task<ServiceResponse<IEnumerable<Sources>>> GetAllSources(GetAllSourcesRequest request);
        Task<ServiceResponse<Sources>> GetSourceById(int sourceId);
        Task<ServiceResponse<bool>> UpdateSourceStatus(int sourceId);
    }
}
