using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;

namespace VisitorManagement_API.Services.Interfaces
{
    public interface IVisitorLogService
    {
        Task<ServiceResponse<string>> AddUpdateVisitorLog(VisitorLog visitorLog);
        Task<ServiceResponse<IEnumerable<VisitorLog>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request);
        Task<ServiceResponse<VisitorLog>> GetVisitorLogById(int visitorId);
        Task<ServiceResponse<bool>> UpdateVisitorLogStatus(int visitorId);
    }
}
