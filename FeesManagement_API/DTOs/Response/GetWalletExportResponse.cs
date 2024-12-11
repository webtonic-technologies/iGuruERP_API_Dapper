namespace FeesManagement_API.DTOs.Responses
{
    public class GetWalletExportResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string FatherName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal WalletBalance { get; set; }
    }
}
