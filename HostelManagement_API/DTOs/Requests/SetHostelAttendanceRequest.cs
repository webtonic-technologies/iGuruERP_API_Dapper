namespace HostelManagement_API.DTOs.Requests
{
    public class SetHostelAttendanceRequest
    {
        public int StudentID { get; set; }
        public int AttendanceTypeID { get; set; }
        public int HostelID { get; set; }
        public int FloorID { get; set; }
        public string AttendanceDate { get; set; }  // 'DD-MM-YYYY' format
        public int StatusID { get; set; }
        public string Remarks { get; set; }
        public int InstituteID { get; set; }
    }
}
