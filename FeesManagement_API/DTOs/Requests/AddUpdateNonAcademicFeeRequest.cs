namespace FeesManagement_API.DTOs.Requests
{
    public class AddUpdateNonAcademicFeeRequest
    {
        //public int NonAcademicFeesID { get; set; }
        public int InstituteID { get; set; }
        public int PayeeTypeID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int? StudentID { get; set; }
        public int? EmployeeID { get; set; }  // New field for Employee ID
        public string? PayeeName { get; set; }  // New field for Payee Name
        public string PaymentDate { get; set; }
        public int FeeHeadID { get; set; }
        public decimal FeeAmount { get; set; }
        public int PaymentModeID { get; set; }
        public string TransactionDetails { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}
