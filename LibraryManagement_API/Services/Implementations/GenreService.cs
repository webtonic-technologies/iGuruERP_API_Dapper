using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Interfaces;

namespace LibraryManagement_API.Services.Implementations
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateGenres(AddUpdateGenreRequest request)
        {
            return await _genreRepository.AddUpdateGenres(request);
        }


        public async Task<ServiceResponse<List<GenreResponse>>> GetAllGenres(GetAllGenreRequest request)
        {
            return await _genreRepository.GetAllGenres(request);
        }

        public async Task<ServiceResponse<Genre>> GetGenreById(int genreId)
        {
            return await _genreRepository.GetGenreById(genreId);
        }

        public async Task<ServiceResponse<bool>> DeleteGenre(int genreId)
        {
            return await _genreRepository.DeleteGenre(genreId);
        }
    }
}
