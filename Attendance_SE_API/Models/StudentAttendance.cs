namespace Attendance_SE_API.Models
{
    public class StudentAttendance
    {
        public int AttendanceID { get; set; }
        public int InstituteID { get; set; }
        public int AttendanceTypeID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int SubjectID { get; set; } // Add this property
        public int TimeSlotTypeID { get; set; } // Add this property
        public bool IsMarkAsHoliday { get; set; } // Add this property
        public int StudentID { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }
    }
}
