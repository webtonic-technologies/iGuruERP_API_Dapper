using EventGallery_API.DTOs.Requests.Approvals;
using EventGallery_API.DTOs.Responses.Approvals;
using EventGallery_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGallery_API.Services.Interfaces
{
    public interface IHolidayApprovalService
    {
        Task<ServiceResponse<List<GetAllHolidaysApprovalsResponse>>> GetAllHolidaysApprovals(GetAllHolidaysApprovalsRequest request);
        Task<bool> UpdateHolidayApprovalStatus(int holidayID, int statusID, int employeeID);
    }
}
