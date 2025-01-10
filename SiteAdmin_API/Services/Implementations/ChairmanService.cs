using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Repository.Implementations;
using SiteAdmin_API.Repository.Interfaces;
using SiteAdmin_API.Services.Interfaces;
using System.Threading.Tasks;

namespace SiteAdmin_API.Services.Implementations
{
    public class ChairmanService : IChairmanService
    {
        private readonly IChairmanRepository _chairmanRepository;

        public ChairmanService(IChairmanRepository chairmanRepository)
        {
            _chairmanRepository = chairmanRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateChairman(AddUpdateChairmanRequest request)
        {
            return await _chairmanRepository.AddUpdateChairman(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetAllChairmanResponse>>> GetAllChairman()
        {
            var response = await _chairmanRepository.GetAllChairman();
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteChairman(DeleteChairmanRequest request)
        {
            var response = await _chairmanRepository.DeleteChairman(request.ChairmanID);
            return response;
        }

        public async Task<ServiceResponse<List<GetInstitutesDDLResponse>>> GetInstitutesDDL()
        {
            return await _chairmanRepository.GetInstitutesDDL();
        }
    }
}
