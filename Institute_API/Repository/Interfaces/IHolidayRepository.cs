using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Repository.Interfaces
{
    public interface IHolidayRepository
    {
        Task<ServiceResponse<int>> AddUpdateHoliday(HolidayDTO holidayDTO);
        Task<ServiceResponse<HolidayDTO>> GetHolidayById(int holidayId);
        Task<ServiceResponse<List<HolidayDTO>>> GetAllHolidays();
        Task<ServiceResponse<List<HolidayDTO>>> GetApprovedHolidays();
        Task<ServiceResponse<bool>> DeleteHoliday(int holidayId);
        Task<ServiceResponse<bool>> UpdateHolidayApprovalStatus(int holidayId, bool isApproved, int approvedBy);
    }
}
