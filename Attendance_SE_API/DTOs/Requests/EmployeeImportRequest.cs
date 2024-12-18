namespace Attendance_SE_API.DTOs.Requests
{
    public class EmployeeImportRequest
    {
        public int InstituteID { get; set; }
        public int DepartmentID { get; set; } 
        public string StartDate { get; set; }  // Format: "dd-MM-yyyy"
        public string EndDate { get; set; }    // Format: "dd-MM-yyyy"
    }
}
