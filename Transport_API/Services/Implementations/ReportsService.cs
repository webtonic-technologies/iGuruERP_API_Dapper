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

        public async Task<ServiceResponse<GetEmployeeTransportationReportResponse>> GetEmployeeTransportationReport(GetReportsRequest request)
        {
            // Fetch the employee transportation report using repository
            return await _reportsRepository.GetEmployeeTransportationReport(request);
        }


        public async Task<ServiceResponse<GetStudentTransportReportResponse>> GetStudentTransportReport(GetReportsRequest request)
        {
            // Fetch the employee transportation report using repository
            return await _reportsRepository.GetStudentTransportReport(request);
        }


        //public async Task<ServiceResponse<IEnumerable<GetStudentTransportReportResponse>>> GetStudentTransportReport(GetReportsRequest request)
        //{
        //    var reportResponse = await _reportsRepository.GetStudentTransportReport(request);

        //    if (reportResponse.Data != null)
        //    {
        //        var reportData = new List<GetStudentTransportReportResponse> { reportResponse.Data };
        //        return new ServiceResponse<IEnumerable<GetStudentTransportReportResponse>>(true, "Record Found", reportData, StatusCodes.Status200OK);
        //    }
        //    else
        //    {
        //        return new ServiceResponse<IEnumerable<GetStudentTransportReportResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
        //    }
        //}

        public async Task<ServiceResponse<IEnumerable<GetTransportAttendanceResponse>>> GetTransportAttendanceReport(TransportAttendanceReportRequest request)
        {
            return await _reportsRepository.GetTransportAttendanceReport(request);
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetDeAllocationReport(GetReportsRequest request)
        {
            return await _reportsRepository.GetDeAllocationReport(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentsReportResponse>>> GetStudentsReport(StudentsReportRequest request)
        {
            return await _reportsRepository.GetStudentsReport(request);
        }


        //Excel Export

        public async Task<byte[]> GetEmployeeTransportationReportExportExcel(GetReportsRequest request)
        {
            return await _reportsRepository.GetEmployeeTransportationReportExportExcel(request);
        }

        public async Task<byte[]> GetStudentTransportReportExportExcel(GetReportsRequest request)
        {
            return await _reportsRepository.GetStudentTransportReportExportExcel(request);
        }

        public async Task<byte[]> GetTransportAttendanceReportExportExcel(TransportAttendanceReportRequest request)
        {
            return await _reportsRepository.GetTransportAttendanceReportExportExcel(request);
        }

        public async Task<byte[]> GetStudentsReportExportExcel(StudentsReportRequest request)
        {
            return await _reportsRepository.GetStudentsReportExportExcel(request);
        }


        // CSV Export Services
        public async Task<byte[]> GetEmployeeTransportationReportExportCSV(GetReportsRequest request)
        {
            return await _reportsRepository.GetEmployeeTransportationReportExportCSV(request);
        }

        public async Task<byte[]> GetStudentTransportReportExportCSV(GetReportsRequest request)
        {
            return await _reportsRepository.GetStudentTransportReportExportCSV(request);
        }

        public async Task<byte[]> GetTransportAttendanceReportExportCSV(TransportAttendanceReportRequest request)
        {
            return await _reportsRepository.GetTransportAttendanceReportExportCSV(request);
        }

        public async Task<byte[]> GetStudentsReportExportCSV(StudentsReportRequest request)
        {
            return await _reportsRepository.GetStudentsReportExportCSV(request);
        }
    }
}
