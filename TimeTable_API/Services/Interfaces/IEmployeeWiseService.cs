using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;

namespace TimeTable_API.Services.Interfaces
{
    public interface IEmployeeWiseService
    {
        Task<ServiceResponse<List<EmployeeWiseResponse>>> GetEmployeeWiseTimetable(EmployeeWiseRequest request);
    }
}
