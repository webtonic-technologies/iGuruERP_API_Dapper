namespace Institute_API.DTOs
{
    public class UpdateHolidayApprovalStatusRequest
    {
        public bool IsApproved { get; set; }
        public int ApprovedBy { get; set; }
        public int holidayId { get; set; }      
    }
}
