namespace FeesManagement_API.DTOs.Requests
{
    public class DailyPaymentSummaryRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int InstituteID { get; set; }
    }
}
