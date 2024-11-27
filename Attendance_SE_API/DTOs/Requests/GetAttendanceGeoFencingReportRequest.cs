namespace Attendance_SE_API.DTOs.Requests
{
    public class GetAttendanceGeoFencingReportRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Date in DD-MM-YYYY format
        public string EndDate { get; set; }    // Date in DD-MM-YYYY format
    }
}
