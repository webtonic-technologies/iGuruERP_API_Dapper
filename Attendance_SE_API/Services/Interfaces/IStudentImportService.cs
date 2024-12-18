
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.ServiceResponse;
using System.Data;
namespace Attendance_SE_API.Services.Interfaces
{
   

    public interface IStudentImportService
    {
        ServiceResponse<DataTable> DownloadAttendanceTemplate(StudentImportRequest request);
        byte[] GenerateExcelFile(DataTable dataTable, DateTime startDate, DateTime endDate);

        // New method to get the status data for the Status sheet
        DataTable GetStatusData();

        // New method to add the Status sheet to the Excel file
        byte[] AddStatusSheetToExcel(byte[] excelFile, DataTable statusData);
         
        Task<ServiceResponse<bool>> ImportAttendanceData(
            int instituteID,
            string classID,
            string sectionID,
            string attendanceDate,
            string studentID,
            string statusID,
            string academicYearCode
        ); 
    }
}
