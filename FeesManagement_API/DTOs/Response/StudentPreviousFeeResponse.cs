namespace FeesManagement_API.DTOs.Response
{ 
        public class StudentPreviousFeeResponse
        {
            public int StudentID { get; set; }
            public string AdmissionNo { get; set; }
            public string StudentName { get; set; }
            public string RollNo { get; set; }
            public int ClassID { get; set; }
            public string ClassName { get; set; }
            public int SectionID { get; set; }
            public string SectionName { get; set; }
            public string ConcessionGroup { get; set; }
            public int FeeGroupID { get; set; }        // New property for FeeGroupID
            public int FeeTenurityID { get; set; }       // New property for FeeTenurityID
            public decimal TotalLateFee { get; set; }    // New property for total late fee 
            public decimal TotalFeeAmount { get; set; }
            public decimal TotalWaiverAmount { get; set; }   // ← new
            public decimal TotalDiscountAmount { get; set; }   // ← new
            public decimal TotalPaidAmount { get; set; }   // ← new
            public decimal TotalBalance { get; set; }   // ← new
            public List<StudentPreviousFeeDetail> FeeDetails { get; set; }
        }

        public class StudentPreviousFeeDetail
    {
            public int FeeHeadID { get; set; }
            public string FeeHead { get; set; }
            public string TenureType { get; set; }
            public decimal Amount { get; set; }
            public decimal LateFee { get; set; } // Add this property
            public decimal WaiverAmount { get; set; }   // ← new
            public decimal DiscountAmount { get; set; }   // ← new
            public decimal PaidAmount { get; set; }   // ← new
            public decimal Balance { get; set; }   // ← new
            public int TenuritySTMID { get; set; }
            public int FeeCollectionSTMID { get; set; }
        } 
}
