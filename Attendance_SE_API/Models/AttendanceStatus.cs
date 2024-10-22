using System.ComponentModel.DataAnnotations;

namespace Attendance_SE_API.Models
{
    public class AttendanceStatus
    {
        public int StatusID { get; set; }

        [Required(ErrorMessage = "Status name cannot be empty")]
        public string StatusName { get; set; } = string.Empty;

        public string ShortName { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int InstituteID { get; set; }
    }
}
