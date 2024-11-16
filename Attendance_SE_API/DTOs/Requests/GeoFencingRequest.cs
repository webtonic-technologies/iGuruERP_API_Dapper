namespace Attendance_SE_API.DTOs.Requests
{
    public class GeoFencingRequest
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int DepartmentID { get; set; }
        public decimal RadiusInMeters { get; set; }
        public int InstituteID { get; set; }
    }

    public class PaginationRequest
    {
        public int InstituteID { get; set; } 
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
} 