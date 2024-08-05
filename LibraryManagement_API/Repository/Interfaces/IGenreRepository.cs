using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;

namespace LibraryManagement_API.Repository.Interfaces
{
    public interface IGenreRepository
    {
        Task<ServiceResponse<List<GenreResponse>>> GetAllGenres(GetAllGenreRequest request);
        Task<ServiceResponse<string>> AddUpdateGenre(Genre request);
        Task<ServiceResponse<Genre>> GetGenreById(int genreId);
        Task<ServiceResponse<bool>> DeleteGenre(int genreId);
    }
}
