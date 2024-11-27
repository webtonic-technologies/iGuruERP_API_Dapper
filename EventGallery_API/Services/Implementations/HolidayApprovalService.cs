using EventGallery_API.DTOs.Requests.Approvals;
using EventGallery_API.DTOs.Responses.Approvals;
using EventGallery_API.DTOs.ServiceResponse;
using EventGallery_API.Repository.Interfaces;
using EventGallery_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGallery_API.Services.Implementations
{
    public class HolidayApprovalService : IHolidayApprovalService
    {
        private readonly IHolidayApprovalRepository _holidayApprovalRepository;

        public HolidayApprovalService(IHolidayApprovalRepository holidayApprovalRepository)
        {
            _holidayApprovalRepository = holidayApprovalRepository;
        }

        public async Task<ServiceResponse<List<GetAllHolidaysApprovalsResponse>>> GetAllHolidaysApprovals(GetAllHolidaysApprovalsRequest request)
        {
            var holidays = await _holidayApprovalRepository.GetAllHolidaysApprovals(request);
            return new ServiceResponse<List<GetAllHolidaysApprovalsResponse>>(
                true,
                holidays.Count > 0 ? "Holidays fetched successfully." : "No holidays found.",
                holidays,
                200
            );
        }

        //public async Task<ServiceResponse<List<GetAllHolidaysApprovalsResponse>>> GetAllHolidaysApprovals(GetAllHolidaysApprovalsRequest request)
        //{
        //    var holidays = await _holidayApprovalRepository.GetAllHolidaysApprovals(request);
        //    return new ServiceResponse<List<GetAllHolidaysApprovalsResponse>>(
        //        true,
        //        holidays.Count > 0 ? "Holidays fetched successfully." : "No holidays found.",
        //        holidays,
        //        200
        //    );
        //}

        public async Task<bool> UpdateHolidayApprovalStatus(int holidayID, int statusID, int employeeID)
        {
            return await _holidayApprovalRepository.UpdateHolidayApprovalStatus(holidayID, statusID, employeeID);
        }
    }
}
