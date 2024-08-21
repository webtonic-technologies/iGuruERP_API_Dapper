using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;

namespace LibraryManagement_API.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<ServiceResponse<List<AuthorResponse>>> GetAllAuthors(GetAllAuthorsRequest request);
        Task<ServiceResponse<List<AuthorFetchResponse>>> GetAllAuthorsFetch(GetAllAuthorsFetchRequest request);

        Task<ServiceResponse<string>> AddUpdateAuthors(AddUpdateAuthorsRequest request);
        Task<ServiceResponse<Author>> GetAuthorById(int authorId);
        Task<ServiceResponse<bool>> DeleteAuthor(int authorId);
    }
}
