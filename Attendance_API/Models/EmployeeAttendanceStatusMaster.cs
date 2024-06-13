using System.ComponentModel.DataAnnotations;

namespace Attendance_API.Models
{
    public class EmployeeAttendanceStatusMaster
    {
        public int Employee_Attendance_Status_id { get; set; }
        [MaxLength(30)]
        public string Employee_Attendance_Status_Type { get; set; }
        [MaxLength(15)]
        public string Short_Name { get; set; }
    }
}
