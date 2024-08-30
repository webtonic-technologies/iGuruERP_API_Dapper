using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Repository.Interfaces
{
    public interface IAuthorRepository
    {
        Task<ServiceResponse<List<AuthorResponse>>> GetAllAuthors(GetAllAuthorsRequest request);
        Task<ServiceResponse<List<AuthorFetchResponse>>> GetAllAuthorsFetch(GetAllAuthorsFetchRequest request);

        Task<ServiceResponse<Author>> GetAuthorById(int authorId);
        Task<ServiceResponse<string>> AddUpdateAuthors(AddUpdateAuthorsRequest request);
        Task<ServiceResponse<bool>> DeleteAuthor(int authorId);
    }
}
