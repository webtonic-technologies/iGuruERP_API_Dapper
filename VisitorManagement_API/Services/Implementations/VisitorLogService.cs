using System.Collections.Generic;
using System.Threading.Tasks;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;
using VisitorManagement_API.Services.Interfaces;

namespace VisitorManagement_API.Services.Implementations
{
    public class VisitorLogService : IVisitorLogService
    {
        private readonly IVisitorLogRepository _visitorLogRepository;

        public VisitorLogService(IVisitorLogRepository visitorLogRepository)
        {
            _visitorLogRepository = visitorLogRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateVisitorLog(VisitorLog visitorLog)
        {
            return await _visitorLogRepository.AddUpdateVisitorLog(visitorLog);
        }

        public async Task<ServiceResponse<IEnumerable<VisitorLog>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            return await _visitorLogRepository.GetAllVisitorLogs(request);
        }

        public async Task<ServiceResponse<VisitorLog>> GetVisitorLogById(int visitorId)
        {
            return await _visitorLogRepository.GetVisitorLogById(visitorId);
        }

        public async Task<ServiceResponse<bool>> UpdateVisitorLogStatus(int visitorId)
        {
            return await _visitorLogRepository.UpdateVisitorLogStatus(visitorId);
        }
    }
}
