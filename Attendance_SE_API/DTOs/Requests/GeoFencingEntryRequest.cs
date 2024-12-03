﻿namespace Attendance_SE_API.DTOs.Requests
{
    public class GeoFencingEntryRequest
    {
        public int EmployeeID { get; set; }
        public string GeoFencingDate { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string Reason { get; set; }
        public int InstituteID { get; set; }
    }

    public class GeoFencingEntryRequest2
    { 
        public int InstituteID { get; set; }
    }
}