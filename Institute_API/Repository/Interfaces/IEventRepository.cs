using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Repository.Interfaces
{
    public interface IEventRepository
    {
        Task<ServiceResponse<int>> AddUpdateEvent(EventDTO eventDto);
        Task<ServiceResponse<bool>> DeleteEvent(int eventId);
        Task<ServiceResponse<bool>> ToggleEventActiveStatus(int eventId, bool isActive, int UserId);
        Task<ServiceResponse<EventDTO>> GetEventById(int eventId);
        Task<ServiceResponse<List<EventDTO>>> GetApprovedEvents();
        Task<ServiceResponse<string>> GetEventAttachmentFileById(int eventId);
    }
}
