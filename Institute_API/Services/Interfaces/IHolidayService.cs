using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IHolidayService
    {
        Task<ServiceResponse<int>> AddUpdateHoliday(HolidayRequestDTO holidayDTO);
        Task<ServiceResponse<HolidayDTO>> GetHolidayById(int holidayId);
        Task<ServiceResponse<List<HolidayDTO>>> GetAllHolidays(CommonRequestDTO commonRequest);
        Task<ServiceResponse<List<HolidayDTO>>> GetApprovedHolidays(CommonRequestDTO commonRequest);
        Task<ServiceResponse<bool>> DeleteHoliday(int holidayId);
        Task<ServiceResponse<bool>> UpdateHolidayApprovalStatus(int holidayId, int Status, int approvedBy);
        Task<ServiceResponse<string>> ExportAllHolidaysToExcel(CommonRequestDTO commonRequest);
        Task<ServiceResponse<string>> ExportApprovedHolidaysToExcel(CommonRequestDTO commonRequest);
    }
}
