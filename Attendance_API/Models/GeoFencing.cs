namespace Attendance_API.Models
{
    public class GeoFencing
    {
        public int Geo_Fencing_id { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int Department_id { get; set; }
        public decimal Radius_In_Meters { get; set; }
        public string Search_Location { get; set; }
    }
}
