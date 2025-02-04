namespace Communication_API.DTOs.Requests.WhatsApp
{

    public class SendWhatsAppStudentRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<StudentWhatsAppMessage> StudentMessages { get; set; }  // Renamed to avoid conflict
        public string WhatsAppDate { get; set; } // Changed to string to match request format
        public int SentBy { get; set; } // Changed to string to match request format
    }

    public class StudentWhatsAppMessage
    {
        public int StudentID { get; set; }
        public string WhatsAppMessage { get; set; }  // Renamed property to avoid conflict
    }
     
}
