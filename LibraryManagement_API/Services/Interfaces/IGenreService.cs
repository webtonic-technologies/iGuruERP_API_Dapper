using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Services.Interfaces
{
    public interface IGenreService
    {
        Task<ServiceResponse<string>> AddUpdateGenres(AddUpdateGenreRequest request);
        Task<ServiceResponse<List<GenreResponse>>> GetAllGenres(GetAllGenreRequest request);
        Task<ServiceResponse<Genre>> GetGenreById(int genreId);
        Task<ServiceResponse<bool>> DeleteGenre(int genreId);
    }
}
