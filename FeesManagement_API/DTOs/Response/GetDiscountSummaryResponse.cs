using System;
using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class GetDiscountSummaryResponse
    {
        public StudentDetail StudentDetail { get; set; }
        public List<DiscountDetail11> Discounts { get; set; } = new List<DiscountDetail11>();
    }

    public class StudentDetail1
    {
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        // Use string type if roll number comes as text from DB;
        // otherwise use int and convert it accordingly.
        public string RollNumber { get; set; }
    }

    public class DiscountDetail11
    {
        public int FeesDiscountID { get; set; }
        // A concatenation of FeeHead and Term (ex: "Tuition Fee - Term 1" or "Registration Fee - Single")
        public string FeeType { get; set; }
        public decimal Discount { get; set; }
        public string GivenBy { get; set; }
        // Formatted as "dd-MM-yyyy at hh:mm tt"
        public string GiveDate { get; set; }
        public string Reason { get; set; }
    }
}
