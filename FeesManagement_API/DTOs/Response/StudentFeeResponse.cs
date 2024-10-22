using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class StudentFeeResponse
    {
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public decimal TotalFeeAmount { get; set; }

        // Property to hold the fee details
        public List<StudentFeeDetail> FeeDetails { get; set; }
    }

    public class StudentFeeDetail
    {
        public string FeeHead { get; set; }
        public string TenureType { get; set; }
        public decimal Amount { get; set; }
    }
}
