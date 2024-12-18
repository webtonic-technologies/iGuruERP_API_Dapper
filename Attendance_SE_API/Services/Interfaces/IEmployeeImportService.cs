using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.ServiceResponse;
using System.Data;
using System.Threading.Tasks;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface IEmployeeImportService
    {
        ServiceResponse<DataTable> DownloadAttendanceTemplate(EmployeeImportRequest request);
        //byte[] GenerateExcelFile(DataTable dataTable);
        byte[] GenerateExcelFile(DataTable dataTable, DateTime startDate, DateTime endDate);

        DataTable GetStatusData();
        byte[] AddStatusSheetToExcel(byte[] excelFile, DataTable statusData);

        Task<ServiceResponse<bool>> ImportAttendanceData(
            int instituteID,
            string departmentID,
            string designationID,
            string attendanceDate,
            string employeeID,
            string statusID
        );
    }
}
