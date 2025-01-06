namespace Attendance_SE_API.DTOs.Response
{
    public class GetAbsentStudentsExportResponse
    {
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string AdmissionNumber { get; set; }
    }
}
