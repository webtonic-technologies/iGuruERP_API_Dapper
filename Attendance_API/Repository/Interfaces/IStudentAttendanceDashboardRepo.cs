using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Repository.Interfaces
{
    public interface IStudentAttendanceDashboardRepo
    {
        Task<ServiceResponse<AttendanceCountsDTO>> GetAttendanceCountsForTodayAsync(int instituteId);
        Task<ServiceResponse<IEnumerable<ClasswiseAttendanceCountsDTO>>> GetClasswiseAttendanceCountsForTodayAsync(int instituteId);
        Task<ServiceResponse<IEnumerable<AbsentStudentDTO>>> GetAbsentStudentsForTodayAsync(int instituteId);
        Task<ServiceResponse<IEnumerable<EmployeeOnLeaveDTO>>> GetEmployeesOnLeaveForTodayAsync(int instituteId);
    }
}
