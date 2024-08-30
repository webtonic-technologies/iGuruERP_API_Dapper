using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;

namespace LibraryManagement_API.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<ServiceResponse<List<PublisherResponse>>> GetAllPublishers(GetAllPublishersRequest request);
        Task<ServiceResponse<List<PublisherFetchResponse>>> GetAllPublishersFetch(GetAllPublishersFetchRequest request);
        Task<ServiceResponse<string>> AddUpdatePublisher(Publisher request);
        Task<ServiceResponse<Publisher>> GetPublisherById(int publisherId);
        Task<ServiceResponse<bool>> DeletePublisher(int publisherId);
    }
}
