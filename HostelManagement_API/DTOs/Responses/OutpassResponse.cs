namespace HostelManagement_API.DTOs.Responses
{
    public class OutpassResponse
    {
        public int OutpassID { get; set; }
        public string OutpassCode { get; set; }
        public DateTime OutpassDate { get; set; }
        public int HostelID { get; set; }
        public int StudentID { get; set; }
        public string RoomNo { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ExpectedArrivalTime { get; set; }
        public DateTime EntryTime { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string UploadFile { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
