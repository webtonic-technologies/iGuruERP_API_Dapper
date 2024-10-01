namespace Attendance_API.DTOs.Requests
{
    public class GetAttendanceRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string AttendanceDate { get; set; } // Keep as string for date validation
        public int InstituteID { get; set; }
        public int? AttendanceTypeID { get; set; } // Make this nullable if you want to allow nulls
        public int? TimeSlotTypeID { get; set; }   // Add this property
        public int? SubjectID { get; set; }         // Add this property
    }


}
