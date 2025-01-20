using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IMealAttendanceRepository
    {
        Task<ServiceResponse<string>> SetMealAttendance(SetMealAttendanceRequest request);
        Task<IEnumerable<GetMealAttendanceResponse>> GetMealAttendance(GetMealAttendanceRequest request);
    }
}
