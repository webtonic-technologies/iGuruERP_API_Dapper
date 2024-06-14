using System.ComponentModel.DataAnnotations;

namespace Attendance_API.Models
{
    public class StudentAttendanceStatus
    {
        public int Student_Attendance_Status_id { get; set; }
        [MaxLength(30)]
        public string Student_Attendance_Status_Type { get; set; }
        [MaxLength(15)]
        public string Short_Name { get; set; }
    }
}
