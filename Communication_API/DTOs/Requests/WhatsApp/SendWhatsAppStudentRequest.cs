namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class SendWhatsAppStudentRequest
    {
        public int GroupID { get; set; }
        public int InstituteID { get; set; }
        public List<int> StudentIDs { get; set; }  // List of student IDs
        public string WhatsAppMessage { get; set; }  // Message to send
        public string WhatsAppDate { get; set; }  // Date in "DD-MM-YYYY" format
    }
}
