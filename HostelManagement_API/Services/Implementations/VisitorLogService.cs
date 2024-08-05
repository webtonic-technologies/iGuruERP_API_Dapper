using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class VisitorLogService : IVisitorLogService
    {
        private readonly IVisitorLogRepository _visitorLogRepository;

        public VisitorLogService(IVisitorLogRepository visitorLogRepository)
        {
            _visitorLogRepository = visitorLogRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateVisitorLog(AddUpdateVisitorLogRequest request)
        {
            var hostelVisitorId = await _visitorLogRepository.AddUpdateVisitorLog(request);
            return new ServiceResponse<int>(true, "Visitor log added/updated successfully", hostelVisitorId, 200);
        }

        public async Task<ServiceResponse<PagedResponse<VisitorLogResponse>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            var visitorLogs = await _visitorLogRepository.GetAllVisitorLogs(request);
            return new ServiceResponse<PagedResponse<VisitorLogResponse>>(true, "Visitor logs retrieved successfully", visitorLogs, 200);
        }

        public async Task<ServiceResponse<VisitorLogResponse>> GetVisitorLogById(int hostelVisitorId)
        {
            var visitorLog = await _visitorLogRepository.GetVisitorLogById(hostelVisitorId);
            if (visitorLog == null)
            {
                return new ServiceResponse<VisitorLogResponse>(false, "Visitor log not found", null, 404);
            }
            return new ServiceResponse<VisitorLogResponse>(true, "Visitor log retrieved successfully", visitorLog, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteVisitorLog(int hostelVisitorId)
        {
            var result = await _visitorLogRepository.DeleteVisitorLog(hostelVisitorId);
            return new ServiceResponse<bool>(result > 0, result > 0 ? "Visitor log deleted successfully" : "Visitor log not found", result > 0, result > 0 ? 200 : 404);
        }
    }
}
