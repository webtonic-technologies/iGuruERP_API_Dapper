namespace Attendance_SE_API.DTOs.Responses
{
    public class GetAbsentStudentsResponse
    {
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string AdmissionNumber { get; set; }
    }
}
