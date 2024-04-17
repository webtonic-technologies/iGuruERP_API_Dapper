using System.ComponentModel.DataAnnotations;

namespace Attendance_API.Models
{
    public class StudentAttendanceStatus
    {
        [Key]
        public int Student_Attendance_Status_id { get; set; }
        [Required]
        [StringLength(1000)]
        public string Student_Attendance_Status_Type { get; set; }
        [Required]
        [StringLength(100)]
        public string Short_Name { get; set; }
    }
}
