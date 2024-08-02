namespace Attendance_API.DTOs
{
    public class SubjectAttendanceFilterParams
    {
        public int AcademicYearId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int InstituteId { get; set; }
        public int SubjectId { get; set; }
    }
}
