using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Repository.Interfaces
{
    public interface IPublisherRepository
    {
        Task<ServiceResponse<List<PublisherResponse>>> GetAllPublishers(GetAllPublishersRequest request);
        Task<ServiceResponse<List<PublisherFetchResponse>>> GetAllPublishersFetch(GetAllPublishersFetchRequest request);

        Task<ServiceResponse<Publisher>> GetPublisherById(int publisherId);
        Task<ServiceResponse<string>> AddUpdatePublisher(Publisher request);
        Task<ServiceResponse<bool>> DeletePublisher(int publisherId);
    }
}
