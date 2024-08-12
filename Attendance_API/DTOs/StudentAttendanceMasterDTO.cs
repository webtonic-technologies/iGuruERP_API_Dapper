namespace Attendance_API.DTOs
{
    public class StudentAttendanceMasterDTO
    {
        public int Student_Attendance_id { get; set; }
        public int Student_id { get; set; }
        public int Student_Attendance_Status_id { get; set; }
        public string Remark { get; set; }
        public DateTime Date { get; set; }
        public bool isHoliday {  get; set; }    
        public int TimeSlot_id {  get; set; }    
        public int Subject_id {  get; set; }    
        public bool isDatewise {  get; set; }    
    }
}
