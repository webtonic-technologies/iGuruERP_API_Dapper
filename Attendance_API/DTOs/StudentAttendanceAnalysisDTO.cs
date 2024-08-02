namespace Attendance_API.DTOs
{
    public class StudentAttendanceAnalysisDTO
    {
        public int student_id { get; set; }
        public string Admission_Number { get; set; }
        public string Student_Name { get; set; }
        public int TotalDays { get; set; }
        public int TotalPresentDays { get; set; }
        public decimal AttendancePercentage { get; set; }
    }
}
