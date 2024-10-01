using System;

namespace Attendance_SE_API.DTOs
{
    public class AttendanceReportRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string StartDate { get; set; } // format: "YYYY-MM-DD"
        public string EndDate { get; set; }   // format: "YYYY-MM-DD"
        public int? AttendanceTypeID { get; set; }
        public int? TimeSlotTypeID { get; set; }
        public int? SubjectID { get; set; }
        public int InstituteID { get; set; }
    }
}
