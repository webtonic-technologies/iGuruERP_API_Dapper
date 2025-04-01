using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class StudentFeeResponse
    {
        public int StudentID { get; set; } 
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string ConcessionGroup { get; set; }
        public int FeeGroupID { get; set; }        // New property for FeeGroupID
        public int FeeTenurityID { get; set; }       // New property for FeeTenurityID
        public decimal TotalLateFee { get; set; }    // New property for total late fee 
        public decimal TotalFeeAmount { get; set; }  
        public List<StudentFeeDetail> FeeDetails { get; set; }
    }

    public class StudentFeeDetail
    {
        public int FeeHeadID { get; set; } 
        public string FeeHead { get; set; }
        public string TenureType { get; set; }
        public decimal Amount { get; set; }
        public decimal LateFee { get; set; } // Add this property

    }
}
