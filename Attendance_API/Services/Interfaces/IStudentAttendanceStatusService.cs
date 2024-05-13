using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Services.Interfaces
{
    public interface IStudentAttendanceStatusService
    {
        Task<ServiceResponse<List<StudentAttendanceStatusDTO>>> GetStudentAttendanceStatusList();
        Task<ServiceResponse<StudentAttendanceStatusDTO>> GetStudentAttendanceStatusById(int Student_Attendance_Status_id);
        Task<ServiceResponse<string>> AddStudentAttendanceStatus(StudentAttendanceStatusDTO request);
        Task<ServiceResponse<string>> UpdateStudentAttendanceStatus(StudentAttendanceStatusDTO request);
        Task<ServiceResponse<string>> DeleteStudentAttendanceStatus(int Student_Attendance_Status_id);
    }
}
