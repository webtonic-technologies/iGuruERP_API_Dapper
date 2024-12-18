using Attendance_SE_API.DTOs.Requests;
using System.Data;

namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IEmployeeImportRepository
    {
        DataTable GetAttendanceData(EmployeeImportRequest request);
        DataTable GetStatusData();

        Task DeleteEmployeeAttendanceData(
            int instituteID,
            string attendanceDate,
            string departmentID,
            string employeeID
        );

        Task InsertEmployeeAttendanceData(
            int instituteID,
            string departmentID,
            string designationID,
            string attendanceDate,
            int attendanceTypeID,
            int timeSlotTypeID,
            bool isMarkAsHoliday,
            string employeeID,
            string statusID,
            string remarks
        );
    }
}
