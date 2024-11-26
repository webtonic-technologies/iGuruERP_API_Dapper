using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.ServiceResponse;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.Repository.Interfaces;
using EventGallery_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGallery_API.Services.Implementations
{
    public class EventApprovalService : IEventApprovalService
    {
        private readonly IEventApprovalRepository _eventApprovalRepository;

        public EventApprovalService(IEventApprovalRepository eventApprovalRepository)
        {
            _eventApprovalRepository = eventApprovalRepository;
        }

        public async Task<ServiceResponse<List<GetAllEventsApprovalsResponse>>> GetAllEventsApprovals(GetAllEventsApprovalsRequest request)
        {
            // Fetch the service response from repository
            var eventsResponse = await _eventApprovalRepository.GetAllEventsApprovals(request);

            // Access the Data property from ServiceResponse to get the actual list of events
            return new ServiceResponse<List<GetAllEventsApprovalsResponse>>(
                eventsResponse.Success,
                eventsResponse.Message,  // Use the message from the ServiceResponse
                eventsResponse.Data,     // Use the Data from the ServiceResponse
                eventsResponse.StatusCode,
                eventsResponse.TotalCount
            );
        }


        //public async Task<ServiceResponse<List<GetAllEventsApprovalsResponse>>> GetAllEventsApprovals(GetAllEventsApprovalsRequest request)
        //{
        //    // Fetch the service response from repository
        //    var eventsResponse = await _eventApprovalRepository.GetAllEventsApprovals(request);

        //    // Check count on the Data property of the response
        //    return new ServiceResponse<List<GetAllEventsApprovalsResponse>>(
        //        true,
        //        eventsResponse.Data != null && eventsResponse.Data.Count > 0 ? "Events fetched successfully." : "No events found.",
        //        eventsResponse.Data,
        //        200
        //    );
        //}


        public async Task<ServiceResponse<bool>> UpdateEventApprovalStatus(UpdateEventApprovalStatusRequest request)
        {
            var isUpdated = await _eventApprovalRepository.UpdateEventApprovalStatus(request);
            return new ServiceResponse<bool>(
                isUpdated,
                isUpdated ? "Event status updated successfully." : "Failed to update status.",
                isUpdated,
                isUpdated ? 200 : 400
            );
        }
    }
}
