using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.ServiceResponse;
using EventGallery_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGallery_API.Services.Interfaces
{
    public interface IEventApprovalService
    {
        Task<ServiceResponse<List<GetAllEventsApprovalsResponse>>> GetAllEventsApprovals(GetAllEventsApprovalsRequest request);
        Task<ServiceResponse<bool>> UpdateEventApprovalStatus(UpdateEventApprovalStatusRequest request);
    }
}
