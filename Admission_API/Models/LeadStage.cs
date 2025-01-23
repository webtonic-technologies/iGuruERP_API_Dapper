namespace Admission_API.Models
{
    public class LeadStage
    {
        public int LeadStageID { get; set; }
        public string StageName { get; set; }
        public string ColorCode { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
        public int IsDefault { get; set; }
    }
}
