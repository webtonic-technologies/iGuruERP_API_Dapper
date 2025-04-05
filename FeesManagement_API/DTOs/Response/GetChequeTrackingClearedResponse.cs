namespace FeesManagement_API.DTOs.Responses
{
    public class GetChequeTrackingClearedResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public ClassSectionC ClassSection { get; set; }
        public string RollNo { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDate { get; set; }          // Formatted as "DD-MM-YYYY"
        public string ChequeClearanceDate { get; set; }   // Formatted as "DD-MM-YYYY"
        public decimal Amount { get; set; }
        public string BankName { get; set; }
    }

    public class ClassSectionC
    {
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
}
