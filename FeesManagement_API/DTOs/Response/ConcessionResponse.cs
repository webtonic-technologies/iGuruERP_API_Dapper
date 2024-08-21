namespace FeesManagement_API.DTOs.Responses
{
    public class ConcessionResponse
    {
        public int ConcessionGroupID { get; set; }
        public string ConcessionGroupType { get; set; }
        public bool IsAmount { get; set; }
        public bool IsPercentage { get; set; }
        public int InstituteID { get; set; }
        public List<ConcessionRuleResponse> ConcessionRules { get; set; }
    }

    public class ConcessionRuleResponse
    {
        public int ConcessionRulesID { get; set; }
        public int FeeHeadID { get; set; }
        public int Amount { get; set; }
    }
}
