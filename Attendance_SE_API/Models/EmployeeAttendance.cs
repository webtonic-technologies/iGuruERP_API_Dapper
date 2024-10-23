namespace Attendance_SE_API.Models
{
    public class EmployeeAttendance
    {
        public int AttendanceID { get; set; }
        public int InstituteID { get; set; }
        public int AttendanceTypeID { get; set; }
        public int DepartmentID { get; set; }
        public string AttendanceDate { get; set; }
        public int TimeSlotTypeID { get; set; }
        public bool IsMarkAsHoliday { get; set; }
        public int EmployeeID { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }
    }
}
