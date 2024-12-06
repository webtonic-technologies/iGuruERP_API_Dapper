namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class SendWhatsAppEmployeeRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<int> EmployeeIDs { get; set; }  // List of employee IDs
        public string WhatsAppMessage { get; set; }  // Message to send
        public string WhatsAppDate { get; set; }  // Date in "DD-MM-YYYY" format
    }
}
