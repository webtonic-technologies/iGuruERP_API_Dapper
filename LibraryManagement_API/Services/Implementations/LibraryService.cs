using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Interfaces;

namespace LibraryManagement_API.Services.Implementations
{
    public class LibraryService : ILibraryService
    {
        private readonly ILibraryRepository _libraryRepository;

        public LibraryService(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateLibrary(AddUpdateLibraryRequest request)
        {
            return await _libraryRepository.AddUpdateLibrary(request);
        }

        public async Task<ServiceResponse<List<LibraryResponse>>> GetAllLibraries(GetAllLibraryRequest request)
        {
            return await _libraryRepository.GetAllLibraries(request);
        }

        public async Task<ServiceResponse<Library>> GetLibraryById(int libraryId)
        {
            return await _libraryRepository.GetLibraryById(libraryId);
        }

        public async Task<ServiceResponse<bool>> DeleteLibrary(int libraryId)
        {
            return await _libraryRepository.DeleteLibrary(libraryId);
        }

        public async Task<ServiceResponse<List<LibraryInchargeResponse>>> GetAllLibraryIncharge(GetAllLibraryInchargeRequest request)
        {
            return await _libraryRepository.GetAllLibraryIncharge(request);
        }

        public async Task<ServiceResponse<List<LibraryFetchResponse>>> GetAllLibraryFetch(GetAllLibraryFetchRequest request)
        {
            return await _libraryRepository.GetAllLibraryFetch(request);
        }
    }
}
