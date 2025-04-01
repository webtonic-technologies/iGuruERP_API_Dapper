using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class DynamicStudentFeeResponse
    {
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string ConcessionGroup { get; set; }  
        public decimal TotalFeeAmount { get; set; }
        // Dictionary to hold fee type (e.g., "Tuition Fee (Single)") and its summed amount.
        public Dictionary<string, decimal> FeeAmounts { get; set; } = new Dictionary<string, decimal>();
    }
}
