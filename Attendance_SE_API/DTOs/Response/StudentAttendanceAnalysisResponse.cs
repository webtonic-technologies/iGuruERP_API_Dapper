namespace Attendance_SE_API.DTOs.Responses
{
    public class StudentAttendanceAnalysisResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public int TotalAttendance { get; set; }
        public int TotalAttended { get; set; }
        public decimal AttendancePercentage { get; set; }
    }
}
