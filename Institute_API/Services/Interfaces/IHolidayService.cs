using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IHolidayService
    {
        Task<ServiceResponse<int>> AddUpdateHoliday(HolidayRequestDTO holidayDTO);
        Task<ServiceResponse<HolidayDTO>> GetHolidayById(int holidayId);
        Task<ServiceResponse<List<HolidayDTO>>> GetAllHolidays(int Institute_id);
        Task<ServiceResponse<List<HolidayDTO>>> GetApprovedHolidays(int Institute_id);
        Task<ServiceResponse<bool>> DeleteHoliday(int holidayId);
        Task<ServiceResponse<bool>> UpdateHolidayApprovalStatus(int holidayId, bool isApproved, int approvedBy);
    }
}
