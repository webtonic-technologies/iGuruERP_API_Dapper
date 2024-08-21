namespace Attendance_API.DTOs
{
    public class SubjectwiseAttendanceReportRequest
    {
        public DateTime Date { get; set; }  
        public int class_id { get; set; }
        public int section_id { get; set; }
        public int Institute_Id { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

}
