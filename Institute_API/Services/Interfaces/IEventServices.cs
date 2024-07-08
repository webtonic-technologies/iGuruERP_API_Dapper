using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Services.Interfaces
{
    public interface IEventServices
    {
        Task<ServiceResponse<bool>> DeleteEvent(int eventId);
        Task<ServiceResponse<int>> AddUpdateEvent(EventRequestDTO eventDto);
        Task<ServiceResponse<EventDTO>> GetEventById(int eventId);
        Task<ServiceResponse<List<EventDTO>>> GetApprovedEvents(int Institute_id);
        Task<ServiceResponse<bool>> ToggleEventActiveStatus(int eventId, bool isActive, int userId);
        Task<ServiceResponse<List<EventDTO>>> GetAllEvents(int  Institute_id);
    }
}
