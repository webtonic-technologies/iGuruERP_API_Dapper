using System.Text;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
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

        public async Task<ServiceResponse<IEnumerable<GetStudentsReportResponse>>> GetStudentsReport(StudentsReportRequest request)
        {
            return await _reportsRepository.GetStudentsReport(request);
        }

        public async Task<ServiceResponse<List<GetTransportationPendingFeeReportResponse>>> GetTransportationPendingFeeReport(int instituteID, int routePlanID)
        {
            // Fetch the report from the repository
            var report = await _reportsRepository.GetTransportationPendingFeeReport(instituteID, routePlanID);

            // Check if the report is null or empty and handle accordingly
            if (report == null || !report.Any())
            {
                // Return a failure response if no data found
                return new ServiceResponse<List<GetTransportationPendingFeeReportResponse>>(
                    false,
                    "No data found.",
                    null,
                    StatusCodes.Status404NotFound,
                    0 // No records found
                );
            }

            // Return the successful response with data
            return new ServiceResponse<List<GetTransportationPendingFeeReportResponse>>(
                true,
                "Operation Successful",
                report,
                StatusCodes.Status200OK,
                report.Count() // Provide the total count of records
            );
        }


        //public async Task<ServiceResponse<IEnumerable<GetDeAllocationReportResponse>>> GetDeAllocationReport(GetDeAllocationReportRequest request)
        //{
        //    var result = await _reportsRepository.GetDeAllocationReport(request.InstituteID, request.StartDate, request.EndDate, request.UserTypeID);

        //    if (result == null || !result.Any())
        //    {
        //        return new ServiceResponse<IEnumerable<GetDeAllocationReportResponse>>(false, "No records found.", null, 404);
        //    }

        //    return new ServiceResponse<IEnumerable<GetDeAllocationReportResponse>>(true, "Data fetched successfully.", result, 200);
        //}

        public async Task<ServiceResponse<IEnumerable<GetDeAllocationReportResponse>>> GetDeAllocationReport(GetDeAllocationReportRequest request)
        { 
            return await _reportsRepository.GetDeAllocationReport(request.InstituteID, request.StartDate, request.EndDate, request.UserTypeID);
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

        public async Task<byte[]> GetTransportationPendingFeeReportExportExcel(TransportationFeeReportExExcelRequest request)
        {
            return await _reportsRepository.GetTransportationPendingFeeReportExportExcel(request);
        }


        public async Task<byte[]> GetDeAllocationReportExportExcel(GetDeAllocationReportExportExcelRequest request)
        {
            return await _reportsRepository.GetDeAllocationReportExportExcel(request);
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

        public async Task<byte[]> GetTransportationPendingFeeReportExportCSV(TransportationFeeReportExExcelRequest request)
        {
            return await _reportsRepository.GetTransportationPendingFeeReportExportCSV(request);
        }

        public async Task<byte[]> GetDeAllocationReportExportCSV(GetDeAllocationReportExportExcelRequest request)
        {
            return await _reportsRepository.GetDeAllocationReportExportCSV(request);
        }

    }
}
