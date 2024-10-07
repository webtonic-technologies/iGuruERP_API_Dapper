using EventGallery_API.DTOs.Requests.Approvals;
using EventGallery_API.DTOs.Responses.Approvals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGallery_API.Repository.Interfaces
{
    public interface IHolidayApprovalRepository
    {
        Task<List<GetAllHolidaysApprovalsResponse>> GetAllHolidaysApprovals(GetAllHolidaysApprovalsRequest request);
        Task<bool> UpdateHolidayApprovalStatus(int holidayID, int statusID, int employeeID);
    }
}
