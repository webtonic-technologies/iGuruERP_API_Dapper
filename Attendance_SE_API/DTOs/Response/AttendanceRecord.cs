namespace Attendance_SE_API.DTOs.Response
{
    public class AttendanceRecord1
    {
        public int StudentID { get; set; }
        public string AdmissionNumber { get; set; }
        public string RollNumber { get; set; }
        public string StudentName { get; set; }
        public string MobileNumber { get; set; }

        public DateTime AttendanceDate { get; set; }
        public string AttendanceStatus { get; set; }
    }

    public class AttendanceRecord2
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } 
        public string EmployeeName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string AttendanceStatus { get; set; }
    }
}
