namespace Attendance_SE_API.DTOs.Response
{
    public class GetGeoFencingResponse
    {
        public int GeoFencingID { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string DepartmentName { get; set; } // Change from int to string for Department Name
        public decimal RadiusInMeters { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
