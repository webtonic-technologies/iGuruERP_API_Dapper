using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;

namespace TimeTable_API.Repository.Interfaces
{
    public interface IEmployeeWorkloadRepository
    {
        Task<ServiceResponse<EmployeeWorkloadResponse>> GetEmployeeWorkload(EmployeeWorkloadRequest request);
    }
}
