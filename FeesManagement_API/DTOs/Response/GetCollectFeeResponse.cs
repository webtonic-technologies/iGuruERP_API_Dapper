using System;
using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class GetCollectFeeResponse
    {
        public List<CollectFeeDetail> CollectFeeDetails { get; set; } = new List<CollectFeeDetail>(); 
    }

    public class CollectFeeDetail
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string RollNumber { get; set; }
        public string FatherName { get; set; }
        public string MobileNumber { get; set; }
        public decimal TotalFee { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public DateTime? LastPaidDate { get; set; }
    }
}
