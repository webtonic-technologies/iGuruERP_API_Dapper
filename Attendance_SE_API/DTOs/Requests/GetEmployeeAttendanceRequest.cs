namespace Attendance_SE_API.DTOs.Requests
{
    public class GetEmployeeAttendanceRequest
    {
        public int DepartmentID { get; set; }
        public string AttendanceDate { get; set; } // Date in DD-MM-YYYY format
        public int InstituteID { get; set; }
        public int TimeSlotTypeID { get; set; }
    }
}
