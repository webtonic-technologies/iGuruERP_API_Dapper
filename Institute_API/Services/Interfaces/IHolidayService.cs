using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IHolidayService
    {
        Task<ServiceResponse<int>> AddUpdateHoliday(HolidayDTO holidayDTO);
        Task<ServiceResponse<HolidayDTO>> GetHolidayById(int holidayId);
        Task<ServiceResponse<List<HolidayDTO>>> GetAllHolidays();
        Task<ServiceResponse<List<HolidayDTO>>> GetApprovedHolidays();
        Task<ServiceResponse<bool>> DeleteHoliday(int holidayId);
        Task<ServiceResponse<bool>> UpdateHolidayApprovalStatus(int holidayId, bool isApproved, int approvedBy);
    }
}
