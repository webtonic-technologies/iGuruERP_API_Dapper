namespace FeesManagement_API.DTOs.Responses
{
    public class GetRefundResponse
    {
        public int RefundID { get; set; }
        public string AcademiceYearCode { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int SectionID { get; set; }
        public bool StudentStatus { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public decimal RefundAmount { get; set; }
        public string RefundDate { get; set; } // Format 'DD-MM-YYYY'
        public int PaymentModeID { get; set; }
        public string PaymentMode { get; set; }
        public string Remarks { get; set; }
        public int InstituteID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public bool IsActive { get; set; }
    }
}
