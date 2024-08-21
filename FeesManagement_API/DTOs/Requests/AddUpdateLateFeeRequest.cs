namespace FeesManagement_API.DTOs.Requests
{
    public class AddUpdateLateFeeRequest
    {
        public int LateFeeRuleID { get; set; }
        public int FeeHeadID { get; set; }
        public int FeeTenurityID { get; set; }
        public DateTime? DueDate { get; set; }
        public int InstituteID { get; set; }
        public List<LateFeeClassSectionMappingRequest> ClassSections { get; set; }
        public List<FeesRuleRequest> FeesRules { get; set; }
    }

    public class LateFeeClassSectionMappingRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }

    public class FeesRuleRequest
    {
        public int MinDays { get; set; }
        public int MaxDays { get; set; }
        public decimal LateFee { get; set; }
        public int PerDay { get; set; }
        public decimal TotalLateFee { get; set; }
        public decimal ConsolidatedAmount { get; set; }
    }
}
