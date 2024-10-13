namespace Transport_API.DTOs.Requests
{
    public class GetTransportAttendanceRequest
    {
        public string AttendanceDate { get; set; } // Date in 'DD-MM-YYYY' format
        public int RouteID { get; set; }           // RoutePlanID
        public int AttendanceTypeID { get; set; }  // TransportAttendanceTypeID
        public int InstituteID { get; set; }       // Institute ID
        public int pageNumber { get; set; }        // Page number for pagination
        public int pageSize { get; set; }          // Page size for pagination
    }
}
