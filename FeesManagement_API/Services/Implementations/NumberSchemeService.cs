using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Implementations
{
    public class NumberSchemeService : INumberSchemeService
    {
        private readonly INumberSchemeRepository _numberSchemeRepository;

        public NumberSchemeService(INumberSchemeRepository numberSchemeRepository)
        {
            _numberSchemeRepository = numberSchemeRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateNumberScheme(AddUpdateNumberSchemeRequest request)
        {
            var result = await _numberSchemeRepository.AddUpdateNumberScheme(request);
            return new ServiceResponse<int>(true, "Number Scheme saved successfully", result, 200);
        }

        public async Task<ServiceResponse<IEnumerable<NumberSchemeResponse>>> GetAllNumberSchemes(GetAllNumberSchemesRequest request)
        {
            var result = await _numberSchemeRepository.GetAllNumberSchemes(request);
            return new ServiceResponse<IEnumerable<NumberSchemeResponse>>(true, "Number Schemes retrieved successfully", result, 200);
        }

        public async Task<ServiceResponse<NumberSchemeResponse>> GetNumberSchemeById(int numberSchemeID)
        {
            var result = await _numberSchemeRepository.GetNumberSchemeById(numberSchemeID);
            return new ServiceResponse<NumberSchemeResponse>(true, "Number Scheme retrieved successfully", result, 200);
        }
         
        public async Task<ServiceResponse<int>> UpdateNumberSchemeStatus(int numberSchemeID)
        {
            var result = await _numberSchemeRepository.UpdateNumberSchemeStatus(numberSchemeID);
            return new ServiceResponse<int>(true, "Number Scheme status updated successfully", result, 200);
        }
    }
}
