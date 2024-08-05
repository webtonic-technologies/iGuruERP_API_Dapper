using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Repository.Interfaces
{
    public interface IHolidayRepository
    {
        Task<ServiceResponse<int>> AddUpdateHoliday(HolidayRequestDTO holidayDTO);
        Task<ServiceResponse<HolidayDTO>> GetHolidayById(int holidayId);
        Task<ServiceResponse<List<HolidayDTO>>> GetAllHolidays(int Institute_id, int Academic_year_id, string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null);
        Task<ServiceResponse<List<HolidayDTO>>> GetApprovedHolidays(int Institute_id, int Academic_year_id, string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null);
        Task<ServiceResponse<bool>> DeleteHoliday(int holidayId);
        Task<ServiceResponse<bool>> UpdateHolidayApprovalStatus(int holidayId, bool isApproved, int approvedBy);
    }
}
