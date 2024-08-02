namespace Attendance_API.DTOs
{
    public class AttendanceFilterParams
    {
        public int AcademicYearId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int InstituteId { get; set; }
    }
}
