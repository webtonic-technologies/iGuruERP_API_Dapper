using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.ServiceResponse;
using System.Data;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface IGeoFencingImportService
    {
        ServiceResponse<DataTable> DownloadGeoFencingTemplate(GeoFencingImportRequest request);
        byte[] GenerateExcelFile(DataTable dataTable, DateTime startDate, DateTime endDate);
        DataTable GetStatusData();
        byte[] AddStatusSheetToExcel(byte[] excelFile, DataTable statusData);
        Task<ServiceResponse<bool>> ImportAttendanceData(
            int instituteID,
            string departmentID, 
            string date,
            string employeeID,
            string clockIn,
            string clockOut
        );
    }
}
