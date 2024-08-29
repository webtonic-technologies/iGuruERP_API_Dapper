using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Models;

namespace Student_API.Services.Interfaces
{
    public interface ITimetableServices
    {
        Task<ServiceResponse<int>> AddUpdateTimeTableGroup(TimeTableGroupDTO request);
        Task<ServiceResponse<List<ResponseTimeTableGroupDTO>>> GetTimeTableGroup(int InstituteId);
        Task<ServiceResponse<TimeTableGroupDTO>> GetTimeTableGroupById(int timetableGroupId);
        Task<ServiceResponse<int>> AddOrUpdateTimeTableDaysPlan(DaysSetupDTO daysSetupDTO);
        Task<ServiceResponse<List<TimeTableDaysPlanDTO>>> GetTimeTableDaysPlan();
        Task<ServiceResponse<List<Timetable>>> GetTimetablesByTimetableGroupId(int timetableGroupId);
        Task<ServiceResponse<int>> AddOrUpdateTimetable(Timetable timetable);
        Task<ServiceResponse<bool>> DeleteTimetableGroup(int timetableGroupId);
        Task<ServiceResponse<List<Timetable>>> GetTimetablesByCriteria(TimetableParam timetableParam);
        Task<ServiceResponse<DaysSetupDTO>> GetDaysSetupById(int daysSetupId);
        Task<ServiceResponse<List<ClassTimetableResponse>>> GetClassTimetableDataForDayAsync(int dayId);
    }
}
