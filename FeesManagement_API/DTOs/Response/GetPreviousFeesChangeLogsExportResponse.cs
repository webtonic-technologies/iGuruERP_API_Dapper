namespace FeesManagement_API.DTOs.Response
{
    public class GetPreviousFeesChangeLogsExportResponse
    {
        public string AdmissionNumber { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public string ConcessionGroup { get; set; }
        public string FeeHead { get; set; }
        public string FeeTenurity { get; set; }
        public decimal TotalFeeAmount { get; set; }
        public decimal DiscountedAmount { get; set; }
        public string DiscountedDateTime { get; set; } // Format: 'dd-MM-yyyy at hh:mm tt'
        public string UserName { get; set; }
    }
}
