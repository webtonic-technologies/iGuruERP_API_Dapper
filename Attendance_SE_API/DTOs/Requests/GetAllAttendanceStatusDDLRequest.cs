using System.ComponentModel.DataAnnotations;

namespace Attendance_SE_API.DTOs.Requests
{
    public class GetAllAttendanceStatusDDLRequest
    {
        [Required(ErrorMessage = "InstituteID is required")]
        public int InstituteID { get; set; }
    }
}
