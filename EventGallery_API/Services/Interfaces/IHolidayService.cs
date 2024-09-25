using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.DTOs.ServiceResponse;


namespace EventGallery_API.Services.Interfaces
{
    public interface IHolidayService
    {
        Task<int> AddUpdateHoliday(HolidayRequest request);
        Task<List<HolidayResponse>> GetAllHolidays(HolidaySearchRequest request);
        Task<HolidayResponse> GetHoliday(int holidayID);
        Task<ServiceResponse<bool>> DeleteHoliday(int holidayID);
        Task<List<HolidayResponse>> GetHolidaysByDateRange(DateRangeRequest request);
    }
}
