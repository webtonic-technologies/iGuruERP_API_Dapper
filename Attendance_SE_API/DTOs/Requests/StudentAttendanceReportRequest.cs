namespace Attendance_SE_API.DTOs.Requests
{
    public class StudentAttendanceReportRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? InstituteID { get; set; }
        //public int? AttendanceTypeID { get; set; }
        public int? TimeSlotTypeID { get; set; }
        //public int? SubjectID { get; set; }
        public string AcademicYearCode { get; set; }
    }
}
