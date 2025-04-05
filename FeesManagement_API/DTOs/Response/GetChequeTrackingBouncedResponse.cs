namespace FeesManagement_API.DTOs.Responses
{
    public class GetChequeTrackingBouncedResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public ClassSectionB ClassSection { get; set; }
        public string RollNo { get; set; }
        public string ChequeNo { get; set; }
        public string BounceDate { get; set; } // Returned as "DD-MM-YYYY"
        public decimal BounceCharges { get; set; }
        public string Reason { get; set; }
    }

    public class ClassSectionB
    {
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
}
