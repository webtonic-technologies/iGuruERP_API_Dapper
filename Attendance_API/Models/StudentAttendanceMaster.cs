namespace Attendance_API.Models
{
    public class StudentAttendanceMaster
    {
        public int Student_Attendance_id { get; set; }
        public int Student_id { get; set; }
        public int Student_Attendance_Status_id { get; set; }
        public string Remark { get; set; }
        public DateTime Date { get; set; }
    }
}
