using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using System.Threading.Tasks;

namespace LibraryManagement_API.Services.Interfaces
{
    public interface ILibraryService
    {
        Task<ServiceResponse<string>> AddUpdateLibrary(AddUpdateLibraryRequest request);
        Task<ServiceResponse<List<LibraryResponse>>> GetAllLibraries(GetAllLibraryRequest request);
        Task<ServiceResponse<Library>> GetLibraryById(int libraryId);
        Task<ServiceResponse<bool>> DeleteLibrary(int libraryId);
        Task<ServiceResponse<List<LibraryInchargeResponse>>> GetAllLibraryIncharge(GetAllLibraryInchargeRequest request);
        Task<ServiceResponse<List<LibraryFetchResponse>>> GetAllLibraryFetch(GetAllLibraryFetchRequest request);

    }
}
