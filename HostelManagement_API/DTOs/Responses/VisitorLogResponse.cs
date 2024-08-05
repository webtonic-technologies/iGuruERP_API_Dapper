namespace HostelManagement_API.DTOs.Responses
{
    public class VisitorLogResponse
    {
        public int HostelVisitorID { get; set; }
        public string VisitorCode { get; set; }
        public string VisitorName { get; set; }
        public int NoOfVisitor { get; set; }
        public string PhoneNo { get; set; }
        public int StudentID { get; set; }
        public int HostelID { get; set; }
        public string RoomNo { get; set; }
        public string RelationshipToStudent { get; set; }
        public DateTime DateOfVisit { get; set; }
        public string Address { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public string PurposeOfVisit { get; set; }
        public string UploadFile { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
