using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IVisitorLogRepository
    {
        Task<int> AddUpdateVisitorLog(AddUpdateVisitorLogRequest request);
        Task<ServiceResponse<IEnumerable<VisitorLogResponse>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request);  
        Task<VisitorLogResponse> GetVisitorLogById(int hostelVisitorId);
        Task<int> DeleteVisitorLog(int hostelVisitorId);
    }
}
