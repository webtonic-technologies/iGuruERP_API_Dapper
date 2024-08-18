namespace Configuration.DTOs.Responses
{
    public class OfferResponse
    {
        public int OfferID { get; set; }
        public string OfferName { get; set; }
        public string AcademicYear { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public bool isAmount { get; set; }
        public bool isPercentage { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public List<OfferFeeHeadMappingResponse> OfferFeeHeadMappings { get; set; }
        public List<OfferFeeTenureMappingResponse> OfferFeeTenureMappings { get; set; }
        public List<OfferClassSectionMappingResponse> OfferClassSectionMappings { get; set; }
    }

    public class OfferFeeHeadMappingResponse
    {
        public int FeeHeadID { get; set; }
        public string FeeHeadName { get; set; }
    }

    public class OfferFeeTenureMappingResponse
    {
        public int FeeTenurityID { get; set; }
        public string FeeTenurityName { get; set; }
    }

    public class OfferClassSectionMappingResponse
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
    }
}
