using System.ComponentModel.DataAnnotations;

namespace Attendance_API.Models
{
    public class StudentAttendanceStatus
    {
        public int Student_Attendance_Status_id { get; set; }
        public string Student_Attendance_Status_Type { get; set; }
        public string Short_Name { get; set; }
    }
}
