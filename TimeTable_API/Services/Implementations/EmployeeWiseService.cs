using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Services.Implementations
{
    public class EmployeeWiseService : IEmployeeWiseService
    {
        private readonly IEmployeeWiseRepository _employeeWiseRepository;

        public EmployeeWiseService(IEmployeeWiseRepository employeeWiseRepository)
        {
            _employeeWiseRepository = employeeWiseRepository;
        }

        public async Task<ServiceResponse<List<EmployeeWiseResponse>>> GetEmployeeWiseTimetable(EmployeeWiseRequest request)
        {
            return await _employeeWiseRepository.GetEmployeeWiseTimetable(request);
        }
    }
}
