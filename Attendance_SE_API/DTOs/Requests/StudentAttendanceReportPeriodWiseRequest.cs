namespace Attendance_SE_API.DTOs.Requests
{
    public class StudentAttendanceReportPeriodWiseRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string Date { get; set; } // Date in string format (could be parsed on backend)
        public int InstituteID { get; set; }
        //public int AttendanceTypeID { get; set; }
        //public int SubjectID { get; set; }
    }
}
