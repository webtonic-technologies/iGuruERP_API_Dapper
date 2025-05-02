namespace FeesManagement_API.DTOs.Response
{
    public class DynamicStudentPreviousFeeResponse
    {
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string ConcessionGroup { get; set; }
        public decimal TotalFeeAmount { get; set; } 
        public Dictionary<string, decimal> FeeAmounts { get; set; } = new Dictionary<string, decimal>();
    }
}
