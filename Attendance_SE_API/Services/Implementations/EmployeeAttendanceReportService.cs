using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Services.Implementations
{
    public class EmployeeAttendanceReportService : IEmployeeAttendanceReportService
    {
        private readonly IEmployeeAttendanceReportRepository _repository;

        public EmployeeAttendanceReportService(IEmployeeAttendanceReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployeeAttendanceReportResponse> GetAttendanceReport(EmployeeAttendanceReportRequest request)
        {
            return await _repository.GetAttendanceReport(request);
        }

         
    }
}
