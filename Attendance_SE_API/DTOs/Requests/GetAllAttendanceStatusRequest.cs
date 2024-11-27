using System.ComponentModel.DataAnnotations;

namespace Attendance_SE_API.DTOs.Requests
{
    public class GetAllAttendanceStatusRequest
    {
        [Required(ErrorMessage = "InstituteID is required")]
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
