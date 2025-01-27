using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;

namespace LibraryManagement_API.Repository.Interfaces
{
    public interface ILibraryRepository
    {
        Task<ServiceResponse<List<LibraryResponse>>> GetAllLibraries(GetAllLibraryRequest request);
        Task<ServiceResponse<string>> AddUpdateLibrary(AddUpdateLibraryRequest request);
        Task<ServiceResponse<LibraryResponse>> GetLibraryById(int libraryId);
        Task<ServiceResponse<bool>> DeleteLibrary(int libraryId);
        Task<ServiceResponse<List<LibraryInchargeResponse>>> GetAllLibraryIncharge(GetAllLibraryInchargeRequest request);
        Task<ServiceResponse<List<LibraryFetchResponse>>> GetAllLibraryFetch(GetAllLibraryFetchRequest request);


    }
}
