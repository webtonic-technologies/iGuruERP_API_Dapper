namespace FeesManagement_API.DTOs.Requests
{
    public class PaymentChecklistSetRequest
    {
        public int InstituteID { get; set; }
        public int FeeHeadID { get; set; }
        public int FeeTenureID { get; set; }
        public bool IsEnable { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsEditable { get; set; }
        public int SequenceOrder { get; set; }
    }
}
