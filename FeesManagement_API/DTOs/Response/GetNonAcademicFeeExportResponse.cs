namespace FeesManagement_API.DTOs.Responses
{
    public class GetNonAcademicFeeExportResponse
    {
        public int NonAcademicFeesID { get; set; }
        public int PayeeTypeID { get; set; }
        public string PayeeType { get; set; }
        public int PayeeID { get; set; }
        public string PayeeName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string Date { get; set; }
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; }
        public decimal FeeAmount { get; set; }
        public int PaymentModeID { get; set; }
        public string PaymentMode { get; set; }
        public string TransactionDetails { get; set; }
        public string Remarks { get; set; }
    }
}
