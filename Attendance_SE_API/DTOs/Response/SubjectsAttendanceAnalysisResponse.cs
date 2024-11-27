namespace Attendance_SE_API.DTOs.Responses
{
    public class SubjectAttendanceDetails
    {
        public int TotalAttendance { get; set; }
        public int TotalAttended { get; set; }
        public decimal AttendancePercentage { get; set; }
    }

    public class SubjectsAttendanceAnalysisResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public Dictionary<string, SubjectAttendanceDetails> Subjects { get; set; }  // Key is SubjectName
    }
}
