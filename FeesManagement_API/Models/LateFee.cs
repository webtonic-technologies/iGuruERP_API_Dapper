namespace FeesManagement_API.Models
{
    public class LateFee
    {
        public int LateFeeID { get; set; }
        public string RuleName { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
