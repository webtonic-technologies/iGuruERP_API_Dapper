namespace Transport_API.DTOs.Response
{
    public class GetTransportAttendanceResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string ClassSection { get; set; }
        public string MobileNo { get; set; } = "-";  // Placeholder, as it's not mentioned how to retrieve this
        public List<DayAttendance> Days { get; set; } = new List<DayAttendance>();
    }

    public class DayAttendance
    {
        public string AttendanceDate { get; set; } // Date in the format 'Jan 07, Sun'
        public string Pickup { get; set; }         // Pickup status
        public string Drop { get; set; }           // Drop status
    }

}
