using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;

namespace Transport_API.Repository.Interfaces
{
    public interface IReportsRepository
    {
        Task<ServiceResponse<IEnumerable<ReportResponse>>> GetTransportPendingFeeReport(GetReportsRequest request);



        // Method to fetch the Employee Transportation report
        Task<ServiceResponse<GetEmployeeTransportationReportResponse>> GetEmployeeTransportationReport(GetReportsRequest request);

        // Method to fetch the vehicle details for a route including driver and coordinator info
        Task<EmployeeVehicleDetails> GetEmployeeVehicleDetails(int routePlanID, int instituteID);

        // Method to get the total number of employees assigned to a route
        Task<int> GetTotalEmployeeCount(int routePlanID, int vehicleID);




        Task<ServiceResponse<GetStudentTransportReportResponse>> GetStudentTransportReport(GetReportsRequest request);

        // Additional methods
        Task<VehicleDetails> GetVehicleDetails(int routePlanID, int instituteID);
        Task<int> GetTotalStudentCount(int routePlanID, int vehicleID);
        Task<IEnumerable<StudentDetails>> GetStudentsForRoute(int routePlanID);

        Task<ServiceResponse<IEnumerable<GetTransportAttendanceResponse>>> GetTransportAttendanceReport(TransportAttendanceReportRequest request); // New method for Transport Attendance Report

        Task<ServiceResponse<IEnumerable<ReportResponse>>> GetDeAllocationReport(GetReportsRequest request);
        Task<ServiceResponse<IEnumerable<GetStudentsReportResponse>>> GetStudentsReport(StudentsReportRequest request);


        //Excel Export

        Task<byte[]> GetEmployeeTransportationReportExportExcel(GetReportsRequest request);
        Task<byte[]> GetStudentTransportReportExportExcel(GetReportsRequest request);
        Task<byte[]> GetTransportAttendanceReportExportExcel(TransportAttendanceReportRequest request);
        Task<byte[]> GetStudentsReportExportExcel(StudentsReportRequest request);

        // CSV Export Methods
        Task<byte[]> GetEmployeeTransportationReportExportCSV(GetReportsRequest request);
        Task<byte[]> GetStudentTransportReportExportCSV(GetReportsRequest request);
        Task<byte[]> GetTransportAttendanceReportExportCSV(TransportAttendanceReportRequest request);
        Task<byte[]> GetStudentsReportExportCSV(StudentsReportRequest request);
    }
}
