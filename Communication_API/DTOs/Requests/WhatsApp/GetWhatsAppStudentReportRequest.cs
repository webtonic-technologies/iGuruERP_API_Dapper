namespace Communication_API.DTOs.Requests.WhatsApp
{
    public class GetWhatsAppStudentReportRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string StartDate { get; set; }  // StartDate as string in 'DD-MM-YYYY' format
        public string EndDate { get; set; }    // EndDate as string in 'DD-MM-YYYY' format
        public int InstituteID { get; set; }
        public string Search { get; set; }
    }
}
