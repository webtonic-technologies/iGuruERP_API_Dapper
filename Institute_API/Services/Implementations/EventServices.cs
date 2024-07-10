using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Services.Interfaces;
using Institute_API.Repository.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class EventServices : IEventServices
    {
        private readonly IEventRepository _eventRepository;
        private readonly IImageService _imageService;
        public EventServices(IEventRepository eventRepository, IImageService imageService)
        {
            _eventRepository = eventRepository;
            _imageService = imageService;
        }
        public async Task<ServiceResponse<int>> AddUpdateEvent(EventRequestDTO eventDto)
        {
            try
            {
                if (eventDto.AttachmentFile != null && eventDto.AttachmentFile != "")
                {
                    var file = await _imageService.SaveImageAsync(eventDto.AttachmentFile, "Event");
                    if (eventDto.Event_id != 0)
                    {
                        var data = await _eventRepository.GetEventAttachmentFileById(eventDto.Event_id);
                        if (data.Data != null)
                        {
                            _imageService.DeleteFile(data.Data);
                        }
                    }
                    eventDto.AttachmentFile = file.relativePath;
                }
                //_imageService.SaveImageAsync();
                return await _eventRepository.AddUpdateEvent(eventDto);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteEvent(int eventId)
        {
            try
            {
                return await _eventRepository.DeleteEvent(eventId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<bool>> ToggleEventActiveStatus(int eventId, bool isActive, int userId)
        {
            try
            {
                return await _eventRepository.ToggleEventActiveStatus(eventId, isActive, userId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<List<EventDTO>>> GetApprovedEvents(CommonRequestDTO commonRequest)
        {
            try
            {
                var data = await _eventRepository.GetApprovedEvents(commonRequest.Institute_id, commonRequest.Academic_year_id, commonRequest.sortColumn, commonRequest.sortDirection, commonRequest.pageSize, commonRequest.pageNumber);
                foreach (var eventDto in data.Data)
                {
                    if (eventDto != null && eventDto.AttachmentFile != null && eventDto.AttachmentFile != "")
                    {
                        eventDto.AttachmentFile = _imageService.GetImageAsBase64(eventDto.AttachmentFile);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EventDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<EventDTO>>> GetAllEvents(CommonRequestDTO commonRequest)
        {
            try
            {
                var data = await _eventRepository.GetAllEvents(commonRequest.Institute_id, commonRequest.Academic_year_id, commonRequest.sortColumn, commonRequest.sortDirection, commonRequest.pageSize, commonRequest.pageNumber);
                foreach (var eventDto in data.Data)
                {
                    if (eventDto != null && eventDto.AttachmentFile != null && eventDto.AttachmentFile != "")
                    {
                        eventDto.AttachmentFile = _imageService.GetImageAsBase64(eventDto.AttachmentFile);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EventDTO>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<EventDTO>> GetEventById(int eventId)
        {
            try
            {
                var data = await _eventRepository.GetEventById(eventId);
                if (data.Data != null && data.Data.AttachmentFile != null && data.Data.AttachmentFile != "")
                {
                    data.Data.AttachmentFile = _imageService.GetImageAsBase64(data.Data.AttachmentFile);
                }
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EventDTO>(false, ex.Message, null, 500);
            }
        }


    }
}
