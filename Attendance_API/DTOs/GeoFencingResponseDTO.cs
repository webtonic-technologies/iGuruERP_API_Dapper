﻿namespace Attendance_API.DTOs
{
    public class GeoFencingResponseDTO
    {
        public IEnumerable<GeoFencingResponse> Data { get; set; }
        public long Total { get; set; }
    }
    public class GeoFencingResponse
    {
        public int Geo_Fencing_id { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int Department_id { get; set; }
        public string Department_Name { get; set; }
        public decimal Radius_In_Meters { get; set; }
        public string Search_Location { get; set; }
    }
}
