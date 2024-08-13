using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class HostelTypeService : IHostelTypeService
    {
        private readonly IHostelTypeRepository _hostelTypeRepository;

        public HostelTypeService(IHostelTypeRepository hostelTypeRepository)
        {
            _hostelTypeRepository = hostelTypeRepository;
        }

        public async Task<IEnumerable<HostelTypeResponse>> GetAllHostelTypes()
        {
            return await _hostelTypeRepository.GetAllHostelTypes();
        }
    }
}
