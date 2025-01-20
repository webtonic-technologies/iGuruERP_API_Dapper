namespace HostelManagement_API.DTOs.Requests
{
    public class GetHostelAttendanceRequest
    {
        public string AttendanceDate { get; set; }  // Format: DD-MM-YYYY
        public int HostelID { get; set; }
        public int FloorID { get; set; }
        public int AttendanceTypeID { get; set; }
        public int InstituteID { get; set; }
    }
}
