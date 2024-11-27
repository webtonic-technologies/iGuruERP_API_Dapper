namespace FeesManagement_API.DTOs.Requests
{
    public class AddRefundRequest
    {
        public int RefundID { get; set; }
        public string AcademiceYearCode { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public bool StudentStatus { get; set; }
        public int StudentID { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundDate { get; set; }
        public int PaymentModeID { get; set; }
        public string Remarks { get; set; }
        public int InstituteID { get; set; }
        public int EmployeeID { get; set; }
        public bool IsActive { get; set; }

        public RefundTransfer? RefundTransfer { get; set; }
        public RefundCheque? RefundCheque { get; set; }
        public RefundCard? RefundCard { get; set; }
        public RefundWallet? RefundWallet { get; set; }
    }

    public class RefundTransfer
    {
        public int RTID { get; set; }
        public int RefundID { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class RefundCheque
    {
        public int RCID { get; set; }
        public int RefundID { get; set; }
        public string ChequeNo { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public DateTime IssueDate { get; set; }
    }

    public class RefundCard
    {
        public int RCDID { get; set; }
        public int RefundID { get; set; }
        public int TransactionID { get; set; }
        public DateTime IssueDate { get; set; }
    }

    public class RefundWallet
    {
        public int RWID { get; set; }
        public int RefundID { get; set; }
        public decimal WalletBalance { get; set; }
    }
}
