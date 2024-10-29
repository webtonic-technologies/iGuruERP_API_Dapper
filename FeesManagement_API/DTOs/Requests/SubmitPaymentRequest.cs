namespace FeesManagement_API.DTOs.Requests
{
    public class SubmitPaymentRequest
    {
        public List<Payment> Payment { get; set; }
        public PaymentTransaction PaymentTransaction { get; set; }
    }

    public class Payment
    {
        public int FeesPaymentID { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int InstituteID { get; set; }
        public int FeeGroupID { get; set; }
        public int FeeHeadID { get; set; }
        public int FeeTenurityID { get; set; }
        public int TenuritySTMID { get; set; }
        public int FeeCollectionSTMID { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentTransaction
    {
        public int TransactionID { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal LateFee { get; set; }
        public decimal Offer { get; set; }
        public int PaymentModeID { get; set; }
        public DateTime? CashTransactionDate { get; set; }
        public int? ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string ChequeBankName { get; set; }
        public string CardTransactionDetail { get; set; }
        public DateTime? CardTransactionDate { get; set; }
        public string OnlineTransactionDetail { get; set; }
        public DateTime? OnlineTransactionDate { get; set; }
        public DateTime? QRDate { get; set; }
        public string ChallanTransactionDetail { get; set; }
        public DateTime? ChallanTransactionDate { get; set; }
        public string Remarks { get; set; }
        //public string PaymentIDs { get; set; }
        public int InstituteID { get; set; }
        public DateTime SysTransactionDate { get; set; }
    }
}
