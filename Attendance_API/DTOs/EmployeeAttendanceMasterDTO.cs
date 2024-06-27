using System.ComponentModel.DataAnnotations;

namespace Attendance_API.DTOs
{
    public class EmployeeAttendanceMasterDTO
    {
        public int Employee_Attendance_Master_id { get; set; }
        public int Employee_id { get; set; }
        public int Employee_Attendance_Status_id { get; set; }
        [MaxLength(200)]
        public string Remarks { get; set; }
        public DateTime Date { get; set; }
    }
}
