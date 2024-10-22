using System.Collections.Generic;

namespace Attendance_SE_API.DTOs.Requests
{
    public class EmployeeSetAttendanceRequest
    {
        public List<AttendanceRecord_EMP> AttendanceRecords { get; set; } = new List<AttendanceRecord_EMP>();
        public int InstituteID { get; set; }
        public int DepartmentID { get; set; }
        public string AttendanceDate { get; set; }
        public int TimeSlotTypeID { get; set; }
        public bool IsMarkAsHoliday { get; set; }

        public class AttendanceRecord_EMP
        {
            public int EmployeeID { get; set; }
            public int StatusID { get; set; }
            public string Remarks { get; set; }
        }
    }
}
