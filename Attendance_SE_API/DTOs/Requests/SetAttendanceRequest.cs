using System.Collections.Generic;

namespace Attendance_SE_API.DTOs.Requests
{
    public class SetAttendanceRequest
    {
        public List<AttendanceRecord> AttendanceRecords { get; set; }
        public int InstituteID { get; set; } // InstituteID
        public int AttendanceTypeID { get; set; } // Attendance Type ID to determine if Date Wise or Subject Wise
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string AttendanceDate { get; set; } // Attendance Date in DD-MM-YYYY format
        public int? SubjectID { get; set; } // Optional, required for Subject Wise
        public int? TimeSlotTypeID { get; set; } // Optional, required for Date Wise
        public bool IsMarkAsHoliday { get; set; } // Is Marked as Holiday
    }

    public class AttendanceRecord
    {
        public int StudentID { get; set; }
        public int StatusID { get; set; } // Attendance Status (e.g., Present, Absent)
        public string Remarks { get; set; }
    }
}
