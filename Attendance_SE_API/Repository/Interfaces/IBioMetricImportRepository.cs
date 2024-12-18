using System.Data;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;

namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IBioMetricImportRepository
    {
        DataTable GetBioMetricData(BioMetricImportRequest request);
        Task InsertBioMetricAttendanceData(
            int instituteID,
            string date,
            string employeeID,
            string clockIn,
            string clockOut
        );
    }
}
