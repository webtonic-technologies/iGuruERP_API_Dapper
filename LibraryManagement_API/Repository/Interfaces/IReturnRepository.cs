using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;

namespace LibraryManagement_API.Repository.Interfaces
{
    public interface IReturnRepository
    {
        Task<ServiceResponse<List<GetReturnStudentBookResponse>>> GetReturnStudentBook(GetReturnStudentBookRequest request);
        Task<ServiceResponse<List<GetReturnEmployeeBookResponse>>> GetReturnEmployeeBook(GetReturnEmployeeBookRequest request);
        Task<ServiceResponse<List<GetReturnGuestBookResponse>>> GetReturnGuestBook(GetReturnGuestBookRequest request);
        Task<ServiceResponse<string>> AddBookReturnCollection(CollectBookRequest request);

    }
}
