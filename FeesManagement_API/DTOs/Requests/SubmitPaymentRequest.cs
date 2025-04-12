
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
        //public string TransactionCode { get; set; }  // New field – common for both tables
        public decimal TotalAmount { get; set; }       // Renamed from PaymentAmount
        public decimal LateFeesAmount { get; set; }      // Renamed from LateFee
        public decimal OfferAmount { get; set; }         // Renamed from Offer
        public bool IsWalletUsed { get; set; }           // New field
        public decimal WalletAmount { get; set; }        // New field
        public decimal PayableAmount { get; set; }         // New field
        public int PaymentModeID { get; set; }
        public int? ChequeNo { get; set; }
        public string ChequeDate { get; set; }           // Still string; parsed to DATE on insert
        public string ChequeBankName { get; set; }
        public string TransactionDetail { get; set; }    // New – used in place of card/online/challan details
        public string QRImage { get; set; }              // New – for QR image URL or base64 string etc.
        public string TransactionDate { get; set; }      // New consolidated transaction date
        public string Remarks { get; set; }
        public int InstituteID { get; set; }
        public string SysTransactionDate { get; set; }   // Still string; parsed to DATE on insert
    }
}


//namespace FeesManagement_API.DTOs.Requests
//{
//    public class SubmitPaymentRequest
//    {
//        public List<Payment> Payment { get; set; }
//        public PaymentTransaction PaymentTransaction { get; set; }
//    }

//    public class Payment
//    {
//        public int FeesPaymentID { get; set; }
//        public int StudentID { get; set; }
//        public int ClassID { get; set; }
//        public int SectionID { get; set; }
//        public int InstituteID { get; set; }
//        public int FeeGroupID { get; set; }
//        public int FeeHeadID { get; set; }
//        public int FeeTenurityID { get; set; }
//        public int TenuritySTMID { get; set; }
//        public int FeeCollectionSTMID { get; set; }
//        public decimal Amount { get; set; }
//    }

//    public class PaymentTransaction
//    {
//        public int TransactionID { get; set; }
//        public decimal PaymentAmount { get; set; }
//        public decimal LateFee { get; set; }
//        public decimal Offer { get; set; }
//        public int PaymentModeID { get; set; }
//        public string CashTransactionDate { get; set; } // Now string
//        public int? ChequeNo { get; set; }
//        public string ChequeDate { get; set; } // Now string
//        public string ChequeBankName { get; set; }
//        public string CardTransactionDetail { get; set; }
//        public string CardTransactionDate { get; set; } // Now string
//        public string OnlineTransactionDetail { get; set; }
//        public string OnlineTransactionDate { get; set; } // Now string
//        public string QRDate { get; set; } // Now string
//        public string ChallanTransactionDetail { get; set; }
//        public string ChallanTransactionDate { get; set; } // Now string
//        public string Remarks { get; set; }
//        public int InstituteID { get; set; }
//        public string SysTransactionDate { get; set; } // Now string
//    }

//}
