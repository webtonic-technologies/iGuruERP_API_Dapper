using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IMealAttendanceService
    {
        Task<ServiceResponse<string>> SetMealAttendance(SetMealAttendanceRequest request);
        Task<ServiceResponse<IEnumerable<GetMealAttendanceResponse>>> GetMealAttendance(GetMealAttendanceRequest request);
    }
}
