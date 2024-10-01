using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;

namespace TimeTable_API.Services.Interfaces
{
    public interface ITimeTableService
    {
        Task<ServiceResponse<int>> AddUpdateTimeTable(AddUpdateTimeTableRequest request);
        Task<ServiceResponse<List<TimeTableResponse>>> GetAllTimeTables(GetAllTimeTablesRequest request);
    }
}
