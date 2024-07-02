using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;
using VisitorManagement_API.Services.Interfaces;

namespace VisitorManagement_API.Services.Implementations
{
    public class SourceService : ISourceService
    {
        private readonly ISourceRepository _sourceRepository;

        public SourceService(ISourceRepository sourceRepository)
        {
            _sourceRepository = sourceRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateSource(Source source)
        {
            return await _sourceRepository.AddUpdateSource(source);
        }

        public async Task<ServiceResponse<IEnumerable<Source>>> GetAllSources(GetAllSourcesRequest request)
        {
            return await _sourceRepository.GetAllSources(request);
        }

        public async Task<ServiceResponse<Source>> GetSourceById(int sourceId)
        {
            return await _sourceRepository.GetSourceById(sourceId);
        }

        public async Task<ServiceResponse<bool>> UpdateSourceStatus(int sourceId)
        {
            return await _sourceRepository.UpdateSourceStatus(sourceId);
        }
    }
}
