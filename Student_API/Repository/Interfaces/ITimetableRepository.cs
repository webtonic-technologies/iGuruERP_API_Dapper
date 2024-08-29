using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Models;

namespace Student_API.Repository.Interfaces
{
    public interface ITimetableRepository
    {
        Task<ServiceResponse<int>> AddUpdateTimeTableGroup(TimeTableGroupDTO timeTableGroupDTO);
        Task<ServiceResponse<TimeTableGroupDTO>> GetTimeTableGroupById(int timetableGroupId);
        Task<ServiceResponse<List<ResponseTimeTableGroupDTO>>> GetAllTimeTableGroups(int InstituteId);
        Task<ServiceResponse<int>> AddOrUpdateTimeTableDaysPlan(DaysSetupDTO daysSetupDTO);
        Task<ServiceResponse<List<TimeTableDaysPlanDTO>>> GetTimeTableDaysPlan(int InstituteId);
        Task<ServiceResponse<int>> AddOrUpdateTimetable(Timetable timetable);
        Task<ServiceResponse<List<Timetable>>> GetTimetablesByTimetableGroupId(int timetableGroupId);
        Task<ServiceResponse<bool>> DeleteTimetableGroup(int timetableGroupId);
        Task<ServiceResponse<List<Timetable>>> GetTimetablesByCriteria(string academicYear, int classId, int sectionId , int InstituteId);
        Task<ServiceResponse<DaysSetupDTO>> GetDaysSetupById(int daysSetupId);
        Task<List<ClassTimetableData>> GetClassTimetableData(int dayId, string academicYear);
    }
}
