using System;
using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class GetWaiverSummaryResponse
    {
        public StudentDetail StudentDetail { get; set; }
        public List<WaiverDetail> Waivers { get; set; } = new List<WaiverDetail>();
    }

    public class StudentDetail
    {
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string RollNumber { get; set; }
    }

    public class WaiverDetail
    {
        // FeeType is a concatenation: FeeHead - Term (e.g., “Tuition Fee - Term 1” or “Registration Fee - Single”)
        public int FeesWaiverID { get; set; }
        public string FeeType { get; set; }
        public decimal Waiver { get; set; }
        public string GivenBy { get; set; }
        // Formatted as "dd-MM-yyyy at hh:mm tt"
        public string GiveDate { get; set; }
        public string Reason { get; set; }
    }
}
