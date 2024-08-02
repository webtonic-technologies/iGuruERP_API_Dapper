using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class InfirmaryService : IInfirmaryService
    {
        private readonly IInfirmaryRepository _infirmaryRepository;

        public InfirmaryService(IInfirmaryRepository infirmaryRepository)
        {
            _infirmaryRepository = infirmaryRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateInfirmary(AddUpdateInfirmaryRequest request)
        {
            return await _infirmaryRepository.AddUpdateInfirmary(request);
        }

        public async Task<ServiceResponse<List<InfirmaryResponse>>> GetAllInfirmary(GetAllInfirmaryRequest request)
        {
            return await _infirmaryRepository.GetAllInfirmary(request);
        }

        public async Task<ServiceResponse<Infirmary>> GetInfirmaryById(int id)
        {
            return await _infirmaryRepository.GetInfirmaryById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteInfirmary(int id)
        {
            return await _infirmaryRepository.DeleteInfirmary(id);
        }
    }
}
