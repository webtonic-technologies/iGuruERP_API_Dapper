using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Models;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using System.Xml;

namespace Student_API.Services.Implementations
{
    public class TimetableServices : ITimetableServices
    {
        private readonly ITimetableRepository _timetableRepository;

        public TimetableServices(ITimetableRepository timetableRepository)
        {
            _timetableRepository = timetableRepository; 
        }
        public async Task<ServiceResponse<int>> AddUpdateTimeTableGroup(TimeTableGroupDTO request)
        {
            try
            {
                var data = await _timetableRepository.AddUpdateTimeTableGroup(request);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<TimeTableGroupDTO>> GetTimeTableGroupById(int timetableGroupId)
        {
            try
            {
                var data = await _timetableRepository.GetTimeTableGroupById(timetableGroupId);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<TimeTableGroupDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<ResponseTimeTableGroupDTO>>> GetTimeTableGroup()
        {
            try
            {
                var data = await _timetableRepository.GetAllTimeTableGroups();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ResponseTimeTableGroupDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddOrUpdateTimeTableDaysPlan(DaysSetupDTO daysSetupDTO)
        {
            try
            {
                var data = await _timetableRepository.AddOrUpdateTimeTableDaysPlan(daysSetupDTO);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<List<TimeTableDaysPlanDTO>>> GetTimeTableDaysPlan()
        {
            try
            {
                var data = await _timetableRepository.GetTimeTableDaysPlan();
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<TimeTableDaysPlanDTO>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddOrUpdateTimetable(Timetable timetable)
        {
            try
            {
                var data = await _timetableRepository.AddOrUpdateTimetable(timetable);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

       public async Task<ServiceResponse<List<Timetable>>> GetTimetablesByTimetableGroupId(int timetableGroupId)
        {
            try
            {
                var data = await _timetableRepository.GetTimetablesByTimetableGroupId(timetableGroupId);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Timetable>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteTimetableGroup(int timetableGroupId)
        {
            try
            {
                var data = await _timetableRepository.DeleteTimetableGroup(timetableGroupId);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<List<Timetable>>> GetTimetablesByCriteria(TimetableParam timetableParam)
        {
            try
            {
                var data = await _timetableRepository.GetTimetablesByCriteria(timetableParam.AcademicYear, timetableParam.ClassId, timetableParam.SectionId);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Timetable>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<DaysSetupDTO>> GetDaysSetupById(int daysSetupId)
        {
            try
            {
                var data = await _timetableRepository.GetDaysSetupById(daysSetupId);
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<DaysSetupDTO>(false, ex.Message, null, 500);
            }
        }
    }
}
