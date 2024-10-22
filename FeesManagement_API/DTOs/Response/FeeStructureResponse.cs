using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class FeeStructureResponse
    {
        public List<FeeDetail> FeeDetails { get; set; }
        public decimal TotalFees { get; set; }
    }

    public class FeeDetail
    {
        public string FeeHead { get; set; }
        public string TenureType { get; set; }
        public decimal Amount { get; set; }
    }
}
