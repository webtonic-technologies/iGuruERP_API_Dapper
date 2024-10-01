using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Response;
using EventGallery_API.DTOs.Responses;
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

        public async Task<ServiceResponse<List<GetAllEventsResponse>>> GetAllEvents(GetAllEventsRequest request)
        {
            var eventsResponse = await _eventRepository.GetAllEvents(request);
            if (eventsResponse != null && eventsResponse.Data != null)
            {
                return new ServiceResponse<List<GetAllEventsResponse>>(true, "Events fetched successfully.", eventsResponse.Data, 200, eventsResponse.TotalCount);
            }
            else
            {
                return new ServiceResponse<List<GetAllEventsResponse>>(false, "No events found.", null, 404);
            }
        }

        public async Task<ServiceResponse<GetAllEventsResponse>> GetEventById(int eventId)
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
