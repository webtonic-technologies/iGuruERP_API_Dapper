﻿using System.ComponentModel.DataAnnotations;

namespace Attendance_API.DTOs
{
    public class GeoFencingDTO
    {
        public int Geo_Fencing_id { get; set; }
        [MaxLength(20)]
        public string Latitude { get; set; }
        [MaxLength(20)]
        public string Longitude { get; set; }
        public int Department_id { get; set; }
        public int InstituteId { get; set; }
        public decimal Radius_In_Meters { get; set; }
        public string Search_Location { get; set; }
    }
}
