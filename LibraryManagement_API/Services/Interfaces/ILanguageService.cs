using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Services.Interfaces
{
    public interface ILanguageService
    {
        Task<ServiceResponse<string>> AddUpdateLanguage(AddUpdateLanguageRequest request);
        Task<ServiceResponse<List<LanguageResponse>>> GetAllLanguages(GetAllLanguageRequest request);
        Task<ServiceResponse<List<LanguageFetchResponse>>> GetAllLanguageFetch(GetAllLanguageFetchRequest request);

        Task<ServiceResponse<Language>> GetLanguageById(int languageId);
        Task<ServiceResponse<bool>> DeleteLanguage(int languageId);
    }
}
