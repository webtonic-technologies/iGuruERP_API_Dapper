using Attendance_SE_API.ServiceResponse;
using System.Threading.Tasks;
using System.Data;
using Attendance_SE_API.DTOs.Requests;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface IBioMetricImportService
    {
        ServiceResponse<DataTable> DownloadBioMetricTemplate(BioMetricImportRequest request);
        byte[] GenerateExcelFile(DataTable dataTable, DateTime startDate, DateTime endDate);
        Task<ServiceResponse<bool>> ImportBioMetricAttendanceData(
            int instituteID, 
            string employeeID,
            string attendanceDate,
            string clockIn,
            string clockOut
        );
    }
}
