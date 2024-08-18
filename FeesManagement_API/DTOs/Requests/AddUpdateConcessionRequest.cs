namespace FeesManagement_API.DTOs.Requests
{
    public class AddUpdateConcessionRequest
    {
        public int ConcessionGroupID { get; set; }
        public string ConcessionGroupType { get; set; }
        public bool IsAmount { get; set; }
        public bool IsPercentage { get; set; }
        public int InstituteID { get; set; }
        public List<ConcessionRuleRequest> ConcessionRules { get; set; }
    }

    public class ConcessionRuleRequest
    {
        public int ConcessionRulesID { get; set; }
        public int FeeHeadID { get; set; }
        public int Amount { get; set; }
    }
}
