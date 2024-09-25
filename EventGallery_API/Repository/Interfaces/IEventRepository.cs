﻿using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Response;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.DTOs.ServiceResponse; // Ensure this is included

namespace EventGallery_API.Repository.Interfaces
{
    public interface IEventRepository
    {
        Task<ServiceResponse<int>> AddUpdateEvent(EventRequest request);
        Task<ServiceResponse<List<GetAllEventsResponse>>> GetAllEvents(GetAllEventsRequest request);
        Task<ServiceResponse<EventResponse>> GetEventById(int eventId);
        Task<ServiceResponse<bool>> DeleteEvent(int eventId);
        Task<ServiceResponse<bool>> ExportAllEvents(); // Ensure return type matches
    }
}
