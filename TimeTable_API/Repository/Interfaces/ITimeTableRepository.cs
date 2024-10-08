using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;

namespace TimeTable_API.Repository.Interfaces
{
    public interface ITimeTableRepository
    {
        Task<ServiceResponse<int>> AddUpdateTimeTable(AddUpdateTimeTableRequest request);
        Task<ServiceResponse<List<TimeTableResponse>>> GetAllTimeTables(GetAllTimeTablesRequest request);
        Task<ServiceResponse<List<EmployeeResponse>>> GetEmployees(GetInstituteRequest request);
        Task<ServiceResponse<List<SubjectResponse>>> GetSubjects(GetInstituteRequest request);


    }
}
