namespace Attendance_SE_API.DTOs.Requests
{
    public class EmployeeAttendanceReportRequest
    {
        public int DepartmentID { get; set; }
        public string StartDate { get; set; } // Date format should be validated in the service/repository
        public string EndDate { get; set; }   // Date format should be validated in the service/repository
        public int? InstituteID { get; set; }
        public int? TimeSlotTypeID { get; set; }
    }
}
