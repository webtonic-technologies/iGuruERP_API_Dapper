using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
using Transport_API.DTOs.ServiceResponse;

namespace Transport_API.Services.Interfaces
{
    public interface IReportsService
    {
        Task<ServiceResponse<IEnumerable<ReportResponse>>> GetTransportPendingFeeReport(GetReportsRequest request);
        Task<ServiceResponse<GetEmployeeTransportationReportResponse>> GetEmployeeTransportationReport(GetReportsRequest request);
        Task<ServiceResponse<GetStudentTransportReportResponse>> GetStudentTransportReport(GetReportsRequest request);

        //Task<ServiceResponse<IEnumerable<GetStudentTransportReportResponse>>> GetStudentTransportReport(GetReportsRequest request);
        Task<ServiceResponse<IEnumerable<GetTransportAttendanceResponse>>> GetTransportAttendanceReport(TransportAttendanceReportRequest request);
        Task<ServiceResponse<IEnumerable<GetStudentsReportResponse>>> GetStudentsReport(StudentsReportRequest request);
        Task<ServiceResponse<List<GetTransportationPendingFeeReportResponse>>> GetTransportationPendingFeeReport(int instituteID, int routePlanID);
        Task<ServiceResponse<List<GetTransportReportResponse>>> GetTransportReport(int instituteID, int classID, int sectionID);
        Task<ServiceResponse<IEnumerable<GetDeAllocationReportResponse>>> GetDeAllocationReport(GetDeAllocationReportRequest request);

        //Excel Export
        Task<byte[]> GetEmployeeTransportationReportExportExcel(GetReportsRequest request);
        Task<byte[]> GetStudentTransportReportExportExcel(GetReportsRequest request);
        Task<byte[]> GetTransportAttendanceReportExportExcel(TransportAttendanceReportRequest request);
        Task<byte[]> GetStudentsReportExportExcel(StudentsReportRequest request);
        Task<byte[]> GetTransportationPendingFeeReportExportExcel(TransportationFeeReportExExcelRequest request);
        Task<byte[]> GetDeAllocationReportExportExcel(GetDeAllocationReportExportExcelRequest request);
        Task<byte[]> GetTransportReportExportExcel(GetTransportReportRequest request);


        // CSV Export Services
        Task<byte[]> GetEmployeeTransportationReportExportCSV(GetReportsRequest request);
        Task<byte[]> GetStudentTransportReportExportCSV(GetReportsRequest request);
        Task<byte[]> GetTransportAttendanceReportExportCSV(TransportAttendanceReportRequest request);
        Task<byte[]> GetStudentsReportExportCSV(StudentsReportRequest request);
        Task<byte[]> GetTransportationPendingFeeReportExportCSV(TransportationFeeReportExExcelRequest request);
        Task<byte[]> GetDeAllocationReportExportCSV(GetDeAllocationReportExportExcelRequest request);
        Task<byte[]> GetTransportReportExportCSV(GetTransportReportRequest request);


    }
}
