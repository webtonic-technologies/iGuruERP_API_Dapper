using Attendance_SE_API.DTOs.Requests;
using System.Data;

namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IGeoFencingImportRepository
    {
        DataTable GetGeoFencingData(GeoFencingImportRequest request);
        DataTable GetStatusData();
        Task InsertGeoFencingAttendanceData(
            int instituteID,
            string departmentID,
            string date,
            string employeeID,
            string clockIn,
            string clockOut
        );
    }
}
