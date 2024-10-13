namespace Transport_API.DTOs.Requests
{
    public class TransportAttendanceRequest
    {
        public int TAID { get; set; }               // TAID from tblTransportAttendance
        public string AttendanceDate { get; set; }  // Date in 'DD-MM-YYYY' format
        public int RoutePlanID { get; set; }        // RoutePlanID from tblTransportAttendance
        public int TransportAttendanceTypeID { get; set; } // TransportAttendanceTypeID from tblTransportAttendance
        public string AttendanceStatus { get; set; } // Status ('Present' or 'Absent')
        public int StudentID { get; set; }          // StudentID from tblTransportAttendance
        public string Remarks { get; set; }         // Optional Remarks
        public int InstituteID { get; set; }  // Required Institute ID field

    }
}
