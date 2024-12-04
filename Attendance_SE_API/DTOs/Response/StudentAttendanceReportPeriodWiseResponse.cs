using System.Collections.Generic;

namespace Attendance_SE_API.DTOs.Responses
{
    public class StudentAttendanceReportPeriodWiseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<AttendanceDetailResponse_PW> Data { get; set; }
        public int StatusCode { get; set; }
        public int TotalCount { get; set; }
    }

    public class AttendanceDetailResponse_PW
    {
        public int StudentID { get; set; }
        public string AdmissionNumber { get; set; }
        public string RollNumber { get; set; }
        public string StudentName { get; set; }
        public string MobileNumber { get; set; }

        //public Dictionary<string, string> Attendance { get; set; } // Subject attendance

        public List<AttendanceSubjectInfo> AttendanceList { get; set; }

    }

    public class AttendanceSubjectInfo
    {
        public string Subject { get; set; }  // Name of the subject
        public string AttendanceStatus { get; set; }  // Attendance status, e.g., "Present", "Absent", "Sick leave"
    }

}
