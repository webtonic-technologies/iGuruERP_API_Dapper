﻿namespace Attendance_API.Models
{
    public class EmployeeAttendanceMaster
    {
        public int Employee_Attendance_Master_id { get; set; }
        public int Employee_id { get; set; }
        public int Employee_Attendance_Status_id { get; set; }
        public string Remarks { get; set; }
        public DateTime Date { get; set; }
    }
}
