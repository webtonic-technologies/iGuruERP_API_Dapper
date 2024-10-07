using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.Repository.Interfaces;
using EventGallery_API.Services.Interfaces;
using EventGallery_API.DTOs.ServiceResponse;


namespace EventGallery_API.Services.Implementations
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _holidayRepository;

        public HolidayService(IHolidayRepository holidayRepository)
        {
            _holidayRepository = holidayRepository;
        }

        public async Task<int> AddUpdateHoliday(HolidayRequest request)
        {
            // The dates are already in the correct format, so just forward the request to the repository.
            return await _holidayRepository.AddUpdateHoliday(request);
        }

        public async Task<List<HolidayResponse>> GetAllHolidays(HolidaySearchRequest request)
        {
            return await _holidayRepository.GetAllHolidays(request.AcademicYearCode, request.InstituteID, request.Search);
        }

        public async Task<HolidayResponse> GetHoliday(int holidayID)
        {
            // Retrieve the holiday details from the repository
            var holiday = await _holidayRepository.GetHoliday(holidayID);

            if (holiday != null)
            {
                // Format the date as DD-MM-YYYY
                holiday.Date = $"{holiday.FromDate:dd-MM-yyyy} to {holiday.ToDate:dd-MM-yyyy}";
            }

            return holiday;
        }


        public async Task<ServiceResponse<bool>> DeleteHoliday(int holidayID)
        {
            var result = await _holidayRepository.DeleteHoliday(holidayID);

            if (result)
            {
                return new ServiceResponse<bool>(true, "Holiday deleted successfully.", true, 200);
            }

            return new ServiceResponse<bool>(false, "Holiday not found.", false, 404);
        }



        public async Task<List<HolidayResponse>> GetHolidaysByDateRange(DateRangeRequest request)
        {
            return await _holidayRepository.GetHolidaysByDateRange(request);
        }

        public async Task<ServiceResponse<byte[]>> ExportAllHolidays()
        {
            return await _holidayRepository.ExportAllHolidays();
        }
    }
}
