namespace Attendance_SE_API.DTOs.Response
{
    public class GeoFencingResponse
    {
        public int GeoFencingID { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int DepartmentID { get; set; }
        public decimal RadiusInMeters { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}