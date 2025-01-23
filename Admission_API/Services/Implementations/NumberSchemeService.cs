using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Admission_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Implementations
{
    public class NumberSchemeService : INumberSchemeService
    {
        private readonly INumberSchemeRepository _repository;

        public NumberSchemeService(INumberSchemeRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddUpdateNumberScheme(NumberScheme request)
        {
            return await _repository.AddUpdateNumberScheme(request);
        }

        public async Task<ServiceResponse<List<NumberSchemeResponse>>> GetAllNumberSchemes(GetAllRequest request)
        {
            return await _repository.GetAllNumberSchemes(request);
        }

        public async Task<ServiceResponse<NumberSchemeResponse>> GetNumberSchemeById(int schemeID)
        {
            return await _repository.GetNumberSchemeById(schemeID);
        }

        public async Task<ServiceResponse<bool>> DeleteNumberScheme(int schemeID)
        {
            return await _repository.DeleteNumberScheme(schemeID);
        }
    }
}
