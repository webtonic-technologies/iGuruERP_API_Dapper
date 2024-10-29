namespace FeesManagement_API.DTOs.Responses
{
    public class ChequeTrackingResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public ClassSection ClassSection { get; set; }
        public string RollNo { get; set; }
        public string ChequeNo { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public DateTime ChequeDate { get; set; }
    }

    public class ClassSection
    {
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
}
