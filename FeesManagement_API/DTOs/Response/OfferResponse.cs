namespace Configuration.DTOs.Responses
{
    public class OfferResponse
    {
        public int OfferID { get; set; }
        public string OfferName { get; set; }
        public string AcademicYear { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public string OpeningDateFormatted { get; set; }
        public string ClosingDateFormatted { get; set; }
        public bool isAmount { get; set; }
        public bool isPercentage { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public List<FeeHeadFeeTenureResponse> FeeHeadFeeTenures { get; set; }
        public List<ClassSectionResponse> ClassSections { get; set; }
    }

    public class FeeHeadFeeTenureResponse
    {
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; }
        public int FeeTenurityID { get; set; }
        public int STMTenurityID { get; set; }
        public int FeeCollectionID { get; set; }
        public string FeeTenure { get; set; }
    }

    public class ClassSectionResponse
    {
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
}
