namespace HostelManagement_API.DTOs.Responses
{
    public class GetMealAttendanceResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string ClassSection { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }
        public string HostelName { get; set; }  // Added HostelName here
        public string RoomName { get; set; }    // Added RoomName here
    }
}
