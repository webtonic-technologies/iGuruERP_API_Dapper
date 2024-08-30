using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class HostelFetchService : IHostelFetchService
    {
        private readonly IHostelFetchRepository _hostelFetchRepository;

        public HostelFetchService(IHostelFetchRepository hostelFetchRepository)
        {
            _hostelFetchRepository = hostelFetchRepository;
        }

        public async Task<IEnumerable<HostelFetchResponse>> GetAllHostels(int instituteId)
        {
            return await _hostelFetchRepository.GetAllHostels(instituteId);
        }
    }
}
