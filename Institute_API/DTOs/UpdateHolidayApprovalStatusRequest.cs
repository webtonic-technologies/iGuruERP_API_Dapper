namespace Institute_API.DTOs
{
    public class UpdateHolidayApprovalStatusRequest
    {
        public int Status { get; set; }
        public int ApprovedBy { get; set; }
        public int holidayId { get; set; }      
    }
}
