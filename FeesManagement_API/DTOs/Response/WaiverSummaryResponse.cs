namespace FeesManagement_API.DTOs.Responses
{
    public class WaiverSummaryResponse
    {
        public string AdmissionNumber { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public string RollNumber { get; set; }
        public string FatherName { get; set; }
        public string FatherMobileNo { get; set; }
        public string MotherName { get; set; }
        public string MotherMobileNo { get; set; }
        public decimal TotalWaiver { get; set; }
    }
}
