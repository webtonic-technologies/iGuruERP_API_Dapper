namespace Attendance_API.DTOs
{
    public class EmployeeAttendanceMasterResponseDTO
    {
        public string Employee_Name { get; set; }
        public int Employee_Id { get; set; }
        public int Department_id { get; set; }
        public string DepartmentName { get; set; }
        public int? Employee_Attendance_Master_id { get; set; }
        public int? Employee_Attendance_Status_id { get; set; }
        public string Remarks { get; set; }
    }
}
