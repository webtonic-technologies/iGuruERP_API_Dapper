namespace Attendance_API.DTOs
{
    public class AbsentStudentDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string AdmissionNumber { get; set; }
    }
}
