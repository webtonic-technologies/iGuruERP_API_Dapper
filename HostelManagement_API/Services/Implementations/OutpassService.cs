using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Implementations;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class OutpassService : IOutpassService
    {
        private readonly IOutpassRepository _outpassRepository;

        public OutpassService(IOutpassRepository outpassRepository)
        {
            _outpassRepository = outpassRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateOutpass(AddUpdateOutpassRequest request)
        {
            var outpassId = await _outpassRepository.AddUpdateOutpass(request);
            return new ServiceResponse<int>(true, "Outpass added/updated successfully", outpassId, 200);
        }

        public async Task<ServiceResponse<IEnumerable<OutpassResponse>>> GetAllOutpass(GetAllOutpassRequest request)
        {
            return await _outpassRepository.GetAllOutpass(request); 
        }

        public async Task<ServiceResponse<OutpassResponse>> GetOutpassById(int outpassId)
        {
            var outpass = await _outpassRepository.GetOutpassById(outpassId);
            if (outpass == null)
            {
                return new ServiceResponse<OutpassResponse>(false, "Outpass not found", null, 404);
            }
            return new ServiceResponse<OutpassResponse>(true, "Outpass retrieved successfully", outpass, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteOutpass(int outpassId)
        {
            var result = await _outpassRepository.DeleteOutpass(outpassId);
            return new ServiceResponse<bool>(result > 0, result > 0 ? "Outpass deleted successfully" : "Outpass not found", result > 0, result > 0 ? 200 : 404);
        }
    }
}
