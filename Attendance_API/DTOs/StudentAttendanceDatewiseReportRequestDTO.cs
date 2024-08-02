namespace Attendance_API.DTOs
{
    public class StudentAttendanceDatewiseReportRequestDTO
    {
        public int class_id {  get; set; }  
        public int section_id {  get; set; }  
        public DateTime StartDate {  get; set; }    
        public DateTime EndDate {  get; set; }    
        public int instituteId {  get; set; }   

    }
}
