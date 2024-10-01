namespace Attendance_API.DTOs.Response
{
    public class AttendanceRecord1
    {
        public int StudentID { get; set; }
        public string AdmissionNumber { get; set; }
        public string RollNumber { get; set; }
        public string StudentName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string AttendanceStatus { get; set; }
    }
}
