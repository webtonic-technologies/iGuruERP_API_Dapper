using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IVisitorLogService
    {
        Task<ServiceResponse<int>> AddUpdateVisitorLog(AddUpdateVisitorLogRequest request); 
        Task<ServiceResponse<IEnumerable<VisitorLogResponse>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request); 
        Task<ServiceResponse<VisitorLogResponse>> GetVisitorLogById(int hostelVisitorId);
        Task<ServiceResponse<bool>> DeleteVisitorLog(int hostelVisitorId);
    }
}
