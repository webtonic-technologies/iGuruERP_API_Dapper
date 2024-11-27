namespace FeesManagement_API.DTOs.Responses
{
    public class ConcessionTypeResponse
    {
        public string AdmissionNumber { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public string RollNumber { get; set; }
        public string ConcessionType { get; set; }
        public string FeeHead { get; set; }
        public decimal AmountPercentage { get; set; }
        public decimal TotalFee { get; set; }
        public decimal TotalConcession { get; set; }
        public decimal FinalBalance { get; set; }
    }
}
