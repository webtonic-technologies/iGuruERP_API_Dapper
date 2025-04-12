namespace FeesManagement_API.DTOs.Responses
{
    public class PaymentChecklistGetResponse
    {
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; }
        public int FeeTenureID { get; set; }
        public string FeeTenure { get; set; }
        public bool IsEnable { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsEditable { get; set; }
        public int SequenceOrder { get; set; }
    }
}
