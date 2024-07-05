using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Interfaces;

namespace Transport_API.Services.Implementations
{
    public class ReportsService : IReportsService
    {
        private readonly IReportsRepository _reportsRepository;

        public ReportsService(IReportsRepository reportsRepository)
        {
            _reportsRepository = reportsRepository;
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetTransportPendingFeeReport(GetReportsRequest request)
        {
            return await _reportsRepository.GetTransportPendingFeeReport(request);
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetEmployeeTransportationReport(GetReportsRequest request)
        {
            return await _reportsRepository.GetEmployeeTransportationReport(request);
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetStudentTransportReport(GetReportsRequest request)
        {
            return await _reportsRepository.GetStudentTransportReport(request);
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetDeAllocationReport(GetReportsRequest request)
        {
            return await _reportsRepository.GetDeAllocationReport(request);
        }
    }
}
