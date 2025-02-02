namespace HostelManagement_API.DTOs.Responses
{
    public class OutpassResponse
    {
        public int OutpassID { get; set; }
        public string OutpassCode { get; set; }
        public string OutpassDate { get; set; }
        public int HostelID { get; set; }
        public int StudentID { get; set; }
        public string RoomNo { get; set; }
        public string DepartureTime { get; set; }
        public string ExpectedArrivalTime { get; set; }
        public string EntryTime { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string UploadFile { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }

        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public string HostelName { get; set; }
        public string RoomName { get; set; }
    }
}
