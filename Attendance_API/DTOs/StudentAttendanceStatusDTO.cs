using System.ComponentModel.DataAnnotations;

namespace Attendance_API.DTOs
{
    public class StudentAttendanceStatusDTO
    {
        public int Student_Attendance_Status_id { get; set; }
        [MaxLength(30)]
        public string Student_Attendance_Status_Type { get; set; } = string.Empty;
        [MaxLength(15)]
        public string Short_Name { get; set; } = string.Empty;
        public int InstituteId {  get; set; }   
    }
}
