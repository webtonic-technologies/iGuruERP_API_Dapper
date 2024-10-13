namespace Transport_API.DTOs.Response
{
    //public class TransportAttendanceResponse
    //{
    //    public int TransportAttendanceId { get; set; }
    //    public int AttendanceTypeId { get; set; }
    //    public int StudentId { get; set; }
    //    public DateTime Date { get; set; }
    //    public string AttendanceStatus { get; set; }
    //}

    public class TransportAttendanceResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string ClassSection { get; set; }
        public string AttendanceStatus { get; set; }
        public string Remarks { get; set; }
    }
}
