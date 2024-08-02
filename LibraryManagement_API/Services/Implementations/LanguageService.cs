using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Interfaces;

namespace LibraryManagement_API.Services.Implementations
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;

        public LanguageService(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateLanguage(Language request)
        {
            return await _languageRepository.AddUpdateLanguage(request);
        }

        public async Task<ServiceResponse<List<LanguageResponse>>> GetAllLanguages(GetAllLanguageRequest request)
        {
            return await _languageRepository.GetAllLanguages(request);
        }

        public async Task<ServiceResponse<Language>> GetLanguageById(int languageId)
        {
            return await _languageRepository.GetLanguageById(languageId);
        }

        public async Task<ServiceResponse<bool>> DeleteLanguage(int languageId)
        {
            return await _languageRepository.DeleteLanguage(languageId);
        }
    }
}
