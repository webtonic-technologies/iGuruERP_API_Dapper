using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Interfaces;

namespace LibraryManagement_API.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateAuthor(Author request)
        {
            return await _authorRepository.AddUpdateAuthor(request);
        }

        public async Task<ServiceResponse<List<AuthorResponse>>> GetAllAuthors(GetAllAuthorsRequest request)
        {
            return await _authorRepository.GetAllAuthors(request);
        }

        public async Task<ServiceResponse<Author>> GetAuthorById(int authorId)
        {
            return await _authorRepository.GetAuthorById(authorId);
        }

        public async Task<ServiceResponse<bool>> DeleteAuthor(int authorId)
        {
            return await _authorRepository.DeleteAuthor(authorId);
        }
    }
}
