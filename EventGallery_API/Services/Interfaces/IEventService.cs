using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Response;
using EventGallery_API.DTOs.ServiceResponse; // Ensure this is included

namespace EventGallery_API.Services.Interfaces
{
    public interface IEventService
    {
        Task<ServiceResponse<int>> AddUpdateEvent(EventRequest request);
        Task<ServiceResponse<List<EventResponse>>> GetAllEvents();
        Task<ServiceResponse<EventResponse>> GetEventById(int eventId);
        Task<ServiceResponse<bool>> DeleteEvent(int eventId);
        Task<ServiceResponse<bool>> ExportAllEvents(); // Ensure the return type matches
    }
}
