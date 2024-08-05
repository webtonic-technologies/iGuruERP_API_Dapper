using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Interfaces;

namespace LibraryManagement_API.Services.Implementations
{
    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepository _publisherRepository;

        public PublisherService(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdatePublisher(Publisher request)
        {
            return await _publisherRepository.AddUpdatePublisher(request);
        }

        public async Task<ServiceResponse<List<PublisherResponse>>> GetAllPublishers(GetAllPublishersRequest request)
        {
            return await _publisherRepository.GetAllPublishers(request);
        }

        public async Task<ServiceResponse<Publisher>> GetPublisherById(int publisherId)
        {
            return await _publisherRepository.GetPublisherById(publisherId);
        }

        public async Task<ServiceResponse<bool>> DeletePublisher(int publisherId)
        {
            return await _publisherRepository.DeletePublisher(publisherId);
        }
    }
}
