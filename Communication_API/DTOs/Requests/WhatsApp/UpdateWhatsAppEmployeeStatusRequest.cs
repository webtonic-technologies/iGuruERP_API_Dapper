namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class UpdateWhatsAppEmployeeStatusRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public int EmployeeID { get; set; }
        public int WhatsAppStatusID { get; set; }  // Status ID: 0 for Pending, 1 for Sent, 2 for Delivered, 3 for Failed
    }
}
