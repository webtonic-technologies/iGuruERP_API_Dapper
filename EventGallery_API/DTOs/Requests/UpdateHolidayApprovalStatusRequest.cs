namespace EventGallery_API.DTOs.Requests.Approvals
{
    public class UpdateHolidayApprovalStatusRequest
    {
        public int HolidayID { get; set; }
        public int StatusID { get; set; }
        public int EmployeeID { get; set; } // Maps to ReviewerID
    }
}
