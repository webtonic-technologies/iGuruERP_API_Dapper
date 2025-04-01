using System;

namespace FeesManagement_API.DTOs.Responses
{
    public class GetFeesChangeLogsResponse
    {
        public int StudentID { get; set; }
        public string AdmissionNumber { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public string ConcessionGroup { get; set; }
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; }
        public int FeeGroupID { get; set; }
        public int FeeTenurityID { get; set; }
        public string FeeTenurity { get; set; }
        public decimal TotalFeeAmount { get; set; }
        public decimal DiscountedAmount { get; set; }
        public string DiscountedDateTime { get; set; }
        public string UserName { get; set; }
    }
}
