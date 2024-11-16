namespace Attendance_SE_API.DTOs.Requests
{
    public class AttendanceSubjectsRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
