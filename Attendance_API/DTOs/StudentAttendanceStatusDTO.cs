namespace Attendance_API.DTOs
{
    public class StudentAttendanceStatusDTO
    {
        public int Student_Attendance_Status_id { get; set; }
        public string Student_Attendance_Status_Type { get; set; } = string.Empty;
        public string Short_Name { get; set; } = string.Empty;
    }
}
