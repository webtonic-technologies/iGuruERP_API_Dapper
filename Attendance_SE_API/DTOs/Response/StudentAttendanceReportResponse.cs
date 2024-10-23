using System.Collections.Generic;

namespace Attendance_SE_API.DTOs.Response
{
    public class StudentAttendanceReportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<AttendanceDetailResponse> Data { get; set; }
        public int StatusCode { get; set; }
        public int TotalCount { get; set; }
    }

    public class AttendanceDetailResponse
    {
        public int StudentID { get; set; }
        public string AdmissionNumber { get; set; }
        public string RollNumber { get; set; }
        public string StudentName { get; set; }
        public string MobileNumber { get; set; }
        public int WorkingDays { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public double AttendancePercentage { get; set; }
        public Dictionary<string, string> Attendance { get; set; }
    }
}
