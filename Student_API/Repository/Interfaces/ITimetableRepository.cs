using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Models;

namespace Student_API.Repository.Interfaces
{
    public interface ITimetableRepository
    {
        Task<ServiceResponse<int>> AddUpdateTimeTableGroup(TimeTableGroupDTO timeTableGroupDTO);
        Task<ServiceResponse<TimeTableGroupDTO>> GetTimeTableGroupById(int timetableGroupId);
        Task<ServiceResponse<List<ResponseTimeTableGroupDTO>>> GetAllTimeTableGroups();
        Task<ServiceResponse<int>> AddTimeTableDaysPlan(DaysSetupDTO daysSetupDTO);
        Task<ServiceResponse<List<TimeTableDaysPlanDTO>>> GetTimeTableDaysPlan();
        Task<ServiceResponse<int>> AddOrUpdateTimetable(Timetable timetable);
        Task<ServiceResponse<List<Timetable>>> GetTimetablesByTimetableGroupId(int timetableGroupId);
        Task<ServiceResponse<bool>> DeleteTimetableGroup(int timetableGroupId);
    }
}
