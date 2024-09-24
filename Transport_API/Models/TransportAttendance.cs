public class TransportAttendance
{
    public int TAID { get; set; } // Assuming this is the primary key
    public int RoutePlanID { get; set; }
    public int AttendanceTypeID { get; set; }
    public int StudentID { get; set; }
    public string AttendanceStatus { get; set; } // Should hold 'P' or 'A'
    public DateTime AttendanceDate { get; set; }
    public string Remarks { get; set; }
}
