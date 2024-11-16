using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;


namespace Attendance_SE_API.Services.Interfaces
{
    public interface IMarkAttendanceService
    {
        Task<ServiceResponse<List<AttendanceTypeResponse>>> GetAttendanceType();
        Task<ServiceResponse<List<TimeSlotTypeResponse>>> GetTimeSlotType();
        Task<ServiceResponse<List<AttendanceSubjectsResponse>>> GetAttendanceSubjects(AttendanceSubjectsRequest request);


    }
}
