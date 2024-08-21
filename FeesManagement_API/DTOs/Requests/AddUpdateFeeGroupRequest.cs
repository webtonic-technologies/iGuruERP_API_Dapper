namespace FeesManagement_API.DTOs.Requests
{
    public class AddUpdateFeeGroupRequest
    {
        public int FeeGroupID { get; set; }
        public string GroupName { get; set; }
        public int FeeHeadID { get; set; }
        public int FeeTenurityID { get; set; }
        public decimal Fee { get; set; }
        public int InstituteID { get; set; }
        public List<FeeGroupClassSectionRequest> ClassSections { get; set; }
        public List<FeeGroupCollectionRequest> FeeCollections { get; set; }
    }

    public class FeeGroupClassSectionRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }

    public class TenuritySingleRequest
    {
        public decimal Amount { get; set; }
    }

    public class TenurityTermRequest
    {
        public List<TermDetailRequest> Terms { get; set; }
    }

    public class TermDetailRequest
    {
        public string TermName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class TenurityMonthlyRequest
    {
        public List<MonthDetailRequest> Months { get; set; }
    }

    public class MonthDetailRequest
    {
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }
}
