using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.Repository.Interfaces;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Services.Implementations
{
    public class TimeTableService : ITimeTableService
    {
        private readonly ITimeTableRepository _timeTableRepository;

        public TimeTableService(ITimeTableRepository timeTableRepository)
        {
            _timeTableRepository = timeTableRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateTimeTable(AddUpdateTimeTableRequest request)
        {
            return await _timeTableRepository.AddUpdateTimeTable(request);
        }

        public async Task<ServiceResponse<List<TimeTableResponse>>> GetAllTimeTables(GetAllTimeTablesRequest request)
        {
            return await _timeTableRepository.GetAllTimeTables(request);
        }

        public async Task<ServiceResponse<List<EmployeeResponse>>> GetEmployees(GetInstituteRequest request)
        {
            return await _timeTableRepository.GetEmployees(request);
        }
        public async Task<ServiceResponse<List<SubjectResponse>>> GetSubjects(GetInstituteRequest request)
        {
            return await _timeTableRepository.GetSubjects(request);
        }

    }
}
