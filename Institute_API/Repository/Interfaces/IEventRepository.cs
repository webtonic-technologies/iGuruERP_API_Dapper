using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Repository.Interfaces
{
    public interface IEventRepository
    {
        Task<ServiceResponse<int>> AddUpdateEvent(EventRequestDTO eventDto);
        Task<ServiceResponse<bool>> DeleteEvent(int eventId);
        Task<ServiceResponse<bool>> ToggleEventActiveStatus(int eventId, bool isActive, int UserId);
        Task<ServiceResponse<EventDTO>> GetEventById(int eventId);
        Task<ServiceResponse<List<EventDTO>>> GetApprovedEvents(int Institute_id, int Academic_year_id, string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null);
        Task<ServiceResponse<string>> GetEventAttachmentFileById(int eventId);
        Task<ServiceResponse<List<EventDTO>>> GetAllEvents(int Institute_id, int Academic_year_id, string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null);
    }
}
