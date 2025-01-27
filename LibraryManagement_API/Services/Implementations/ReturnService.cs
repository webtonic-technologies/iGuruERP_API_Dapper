using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Interfaces;

namespace LibraryManagement_API.Services.Implementations
{
    public class ReturnService : IReturnService
    {
        private readonly IReturnRepository _returnRepository;

        public ReturnService(IReturnRepository returnRepository)
        {
            _returnRepository = returnRepository;
        }

        public async Task<ServiceResponse<List<GetReturnStudentBookResponse>>> GetReturnStudentBook(GetReturnStudentBookRequest request)
        {
            return await _returnRepository.GetReturnStudentBook(request);
        }

        public async Task<ServiceResponse<List<GetReturnEmployeeBookResponse>>> GetReturnEmployeeBook(GetReturnEmployeeBookRequest request)
        {
            return await _returnRepository.GetReturnEmployeeBook(request);
        } 

        public async Task<ServiceResponse<List<GetReturnGuestBookResponse>>> GetReturnGuestBook(GetReturnGuestBookRequest request)
        {
            return await _returnRepository.GetReturnGuestBook(request);
        }

        public async Task<ServiceResponse<string>> CollectBook(CollectBookRequest request)
        { 
            return await _returnRepository.AddBookReturnCollection(request); 
        }
    }
}
