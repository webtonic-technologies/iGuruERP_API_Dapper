using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Response;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.DTOs.ServiceResponse; // Ensure this is included

namespace EventGallery_API.Repository.Interfaces
{
    public interface IEventRepository
    {
        Task<ServiceResponse<int>> AddUpdateEvent(EventRequest request);
        Task<ServiceResponse<List<GetAllEventsResponse>>> GetAllEvents(GetAllEventsRequest request);
        Task<int> GetTotalEventCount(GetAllEventsRequest request);

        Task<ServiceResponse<GetAllEventsResponse>> GetEventById(int eventId); // Make sure this matches
        Task<ServiceResponse<bool>> DeleteEvent(int eventId);
        Task<ServiceResponse<byte[]>> ExportAllEvents();

    }
}
