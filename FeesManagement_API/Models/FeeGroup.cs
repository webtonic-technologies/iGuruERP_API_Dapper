namespace FeesManagement_API.Models
{
    public class FeeGroup
    {
        public int FeeGroupID { get; set; }
        public string GroupName { get; set; }
        public int FeeHeadID { get; set; }
        public int FeeTenurityID { get; set; }
        public decimal Fee { get; set; }
        public int InstituteID { get; set; }
    }
}
