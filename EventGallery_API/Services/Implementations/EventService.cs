using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Response;
using EventGallery_API.DTOs.ServiceResponse; // Ensure this is included
using EventGallery_API.Repository.Interfaces;
using EventGallery_API.Services.Interfaces;

namespace EventGallery_API.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateEvent(EventRequest request)
        {
            return await _eventRepository.AddUpdateEvent(request);
        }

        public async Task<ServiceResponse<List<EventResponse>>> GetAllEvents()
        {
            return await _eventRepository.GetAllEvents();
        }

        public async Task<ServiceResponse<EventResponse>> GetEventById(int eventId)
        {
            return await _eventRepository.GetEventById(eventId);
        }

        public async Task<ServiceResponse<bool>> DeleteEvent(int eventId)
        {
            return await _eventRepository.DeleteEvent(eventId);
        }

        public async Task<ServiceResponse<bool>> ExportAllEvents() // Ensure the return type matches
        {
            return await _eventRepository.ExportAllEvents();
        }
    }
}
