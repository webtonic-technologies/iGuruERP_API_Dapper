namespace Attendance_API.DTOs
{
    public class StudentAttendanceMasterRequestDTO
    {
        public int class_id { get; set; }
        public int section_id { get; set; }
        public DateTime Date {  get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
    }
}
