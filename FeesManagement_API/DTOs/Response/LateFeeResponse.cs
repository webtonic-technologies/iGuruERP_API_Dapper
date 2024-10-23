namespace FeesManagement_API.DTOs.Responses
{
    public class LateFeeResponse
    {
        public int LateFeeRuleID { get; set; }
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; }
        public int FeeTenurityID { get; set; }
        public string FeeTenurity { get; set; }
        public DateTime DueDate { get; set; }
        public int InstituteID { get; set; }
        public List<FeesRuleResponse> FeesRules { get; set; } // List of fee rules associated with this late fee
        public List<ClassSectionResponse> ClassSections { get; set; } // List of class sections associated with this late fee
    }

    public class FeesRuleResponse
    {
        public int FeeRulesID { get; set; }
        public int LateFeeRuleID { get; set; }
        public int MinDays { get; set; }
        public int MaxDays { get; set; }
        public decimal LateFee { get; set; }
        public int PerDay { get; set; }
        public decimal TotalLateFee { get; set; }
        public decimal ConsolidatedAmount { get; set; }
    }

    public class ClassSectionResponse
    {
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
}
