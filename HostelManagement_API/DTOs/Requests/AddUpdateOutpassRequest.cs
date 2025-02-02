namespace HostelManagement_API.DTOs.Requests
{
    public class AddUpdateOutpassRequest
    {
        public int? OutpassID { get; set; }
        public string OutpassCode { get; set; }
        public string OutpassDate { get; set; }
        public int HostelID { get; set; }
        public int StudentID { get; set; }
        public string RoomNo { get; set; }
        public string DepartureTime { get; set; }  // Changed to string
        public string ExpectedArrivalTime { get; set; }  // Changed to string
        public string EntryTime { get; set; }  // Changed to string 
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string UploadFile { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
