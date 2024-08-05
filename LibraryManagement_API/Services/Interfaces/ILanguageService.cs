using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;

namespace LibraryManagement_API.Services.Interfaces
{
    public interface ILanguageService
    {
        Task<ServiceResponse<List<LanguageResponse>>> GetAllLanguages(GetAllLanguageRequest request);
        Task<ServiceResponse<string>> AddUpdateLanguage(Language request);
        Task<ServiceResponse<Language>> GetLanguageById(int languageId);
        Task<ServiceResponse<bool>> DeleteLanguage(int languageId);
    }
}
