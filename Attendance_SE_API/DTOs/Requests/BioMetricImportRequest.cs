namespace Attendance_SE_API.DTOs.Requests
{
    public class BioMetricImportRequest
    {
        public int InstituteID { get; set; } 
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
