namespace FeesManagement_API.DTOs.Responses
{
    public class ConcessionResponse
    {
        public int ConcessionGroupID { get; set; }
        public string ConcessionGroupType { get; set; }
        public bool IsAmount { get; set; }
        public bool IsPercentage { get; set; }
        public int InstituteID { get; set; }
        public List<ConcessionRuleResponse> ConcessionRules { get; set; } = new List<ConcessionRuleResponse>();
    }

    public class ConcessionRuleResponse
    {
        public int ConcessionRulesID { get; set; }
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; } // Name of the FeeHead
        public int FeeTenurityID { get; set; } // FeeTenurityID
        public int STMTenurityID { get; set; } // STMTenurityID
        public int FeeCollectionID { get; set; } // FeeCollectionID
        public string FeeTenure { get; set; } // Dynamic FeeTenure value based on FeeTenurityID
        public int Amount { get; set; }
    }
}
