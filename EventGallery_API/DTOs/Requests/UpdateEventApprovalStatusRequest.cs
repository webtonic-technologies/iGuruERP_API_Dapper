namespace EventGallery_API.DTOs.Requests
{
    public class UpdateEventApprovalStatusRequest
    {
        public int EventID { get; set; }
        public int StatusID { get; set; } // StatusID column in tblEvent table
        public int EmployeeID { get; set; } // ReviewerID column in tblEvent table
    }
}
