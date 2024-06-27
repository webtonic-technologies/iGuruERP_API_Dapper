namespace Attendance_API.DTOs
{
    public class StudentAttendanceMasterResponseDTO
    {
        public IEnumerable<StudentAttendanceMasterResponse> Data { get; set; }
        public long Total { get; set; }
    }
    public class StudentAttendanceMasterResponse
    {
        public int? Student_Attendance_id { get; set; }
        public int Student_id { get; set; }
        public string Student_Name { get; set; }
        public string Admission_Number { get; set; }
        public string Roll_Number { get; set; }
        public int? Student_Attendance_Status_id { get; set; }
        public string? Student_Attendance_Status_Type { get; set; }
        public string? Student_Attendance_Status_Short_Name { get; set; }
        public string? Remark { get; set; }
        public DateTime? Date { get; set; }
    }
}
