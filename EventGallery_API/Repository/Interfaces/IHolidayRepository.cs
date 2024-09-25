﻿using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;

namespace EventGallery_API.Repository.Interfaces
{
    public interface IHolidayRepository
    {
        Task<int> AddUpdateHoliday(HolidayRequest request);
        Task<List<HolidayResponse>> GetAllHolidays(int academicYearID, int instituteID, string search);
        Task<HolidayResponse> GetHoliday(int holidayID);
        Task<bool> DeleteHoliday(int holidayID);
        Task<List<HolidayResponse>> GetHolidaysByDateRange(DateRangeRequest request);
    }
}
