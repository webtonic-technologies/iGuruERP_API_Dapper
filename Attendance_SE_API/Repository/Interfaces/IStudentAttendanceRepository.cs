using Attendance_SE_API.Models;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IStudentAttendanceRepository
    {
        Task<List<StudentAttendanceResponse>> GetAttendance(GetAttendanceRequest request);
        Task<bool> SetAttendance(SetAttendanceRequest request);
    }
}
