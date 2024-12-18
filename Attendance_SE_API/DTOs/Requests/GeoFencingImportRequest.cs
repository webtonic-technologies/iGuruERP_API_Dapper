namespace Attendance_SE_API.DTOs.Requests
{
    public class GeoFencingImportRequest
    {
        public int InstituteID { get; set; }
        public int DepartmentID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
