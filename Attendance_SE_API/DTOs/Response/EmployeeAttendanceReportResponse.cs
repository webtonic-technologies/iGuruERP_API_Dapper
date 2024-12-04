using System.Collections.Generic;

namespace Attendance_SE_API.DTOs.Response
{
    public class EmployeeAttendanceReportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<AttendanceDetailResponse_EMP> Data { get; set; }
        public int StatusCode { get; set; }
        public int TotalCount { get; set; }
    }

    public class AttendanceDetailResponse_EMP
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string MobileNumber { get; set; }
        public int WorkingDays { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public double AttendancePercentage { get; set; }
        //public Dictionary<string, string> Attendance { get; set; }
        public List<AttendanceDateInfo1> AttendanceList { get; set; }

    }

    public class AttendanceDateInfo1
    {
        public string AttendanceDate { get; set; }  // The date in "Dec 01" format
        public string AttendanceDay { get; set; }   // The day of the week, e.g., "Sun"
        public string AttendanceStatus { get; set; } // The status, e.g., "Present", "Absent", "-"
    }
}
