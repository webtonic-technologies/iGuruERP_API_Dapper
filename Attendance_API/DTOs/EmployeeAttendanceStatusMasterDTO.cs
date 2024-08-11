using System.ComponentModel.DataAnnotations;

namespace Attendance_API.DTOs
{
    public class EmployeeAttendanceStatusMasterDTO
    {
        public int Employee_Attendance_Status_id { get; set; }
        [MaxLength(30)]
        public string Employee_Attendance_Status_Type { get; set; }
        [MaxLength(15)]
        public string Short_Name { get; set; }
        public int InstituteId { get; set; }
    }
}
