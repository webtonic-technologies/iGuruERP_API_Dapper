using System.Collections.Generic;

namespace Attendance_SE_API.DTOs.Requests
{
    public class SetAttendanceRequest_Employee
    {
        public List<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        public int InstituteID { get; set; }
        public int AttendanceTypeID { get; set; }
        public int DepartmentID { get; set; } // Department ID
        public string AttendanceDate { get; set; } // Attendance date in string format (e.g., "YYYY-MM-DD")
        public int TimeSlotTypeID { get; set; } // Time slot type ID
        public bool IsMarkAsHoliday { get; set; } // Indicates if it is marked as a holiday

        public class AttendanceRecord
        {
            public int EmployeeID { get; set; } // Employee ID
            public int StatusID { get; set; } // Attendance Status ID
            public string Remarks { get; set; } // Remarks for the attendance
        }
    }
}
