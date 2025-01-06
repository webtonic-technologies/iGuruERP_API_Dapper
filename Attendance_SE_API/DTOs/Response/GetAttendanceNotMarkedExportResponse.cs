namespace Attendance_SE_API.DTOs.Response
{
    public class GetAttendanceNotMarkedExportResponse
    {
        public string Class { get; set; }
        public string Section { get; set; }
        public int SectionStrength { get; set; }
        public string ClassTeacher { get; set; }
    }

}
