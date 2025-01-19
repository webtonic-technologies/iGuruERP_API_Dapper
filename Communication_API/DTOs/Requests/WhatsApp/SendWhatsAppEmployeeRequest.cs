namespace Communication_API.DTOs.Requests.WhatsApp
{  
    public class SendWhatsAppEmployeeRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<EmployeeWhatsAppMessage> EmployeeMessages { get; set; }  // Renamed property to EmployeeMessages
        public string WhatsAppDate { get; set; } // Changed to string to match request format
    }
    
    public class EmployeeWhatsAppMessage
    {
        public int EmployeeID { get; set; }
        public string Message { get; set; }  // Renamed property to Message
    } 
    
}
