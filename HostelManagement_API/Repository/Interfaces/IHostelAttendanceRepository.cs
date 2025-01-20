using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IHostelAttendanceRepository
    {
        Task<ServiceResponse<string>> SetHostelAttendance(SetHostelAttendanceRequest request);
        Task<IEnumerable<GetHostelAttendanceResponse>> GetHostelAttendance(GetHostelAttendanceRequest request);
        Task<IEnumerable<GetHostelAttendanceTypeResponse>> GetHostelAttendanceTypes();
        Task<IEnumerable<GetHostelAttendanceStatusResponse>> GetHostelAttendanceStatuses();

    }
}
