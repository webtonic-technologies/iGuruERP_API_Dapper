using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventGallery_API.DTOs.ServiceResponse;

namespace EventGallery_API.Repository.Interfaces
{
    public interface IEventApprovalRepository
    {
        //Task<ServiceResponse<List<GetAllEventsApprovalsResponse>>> GetAllEventsApprovals(GetAllEventsApprovalsRequest request);
        Task<ServiceResponse<List<GetAllEventsApprovalsResponse>>> GetAllEventsApprovals(GetAllEventsApprovalsRequest request);

        Task<bool> UpdateEventApprovalStatus(UpdateEventApprovalStatusRequest request);
    }
}
