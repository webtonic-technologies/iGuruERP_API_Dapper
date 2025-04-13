using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class SectionWiseFee
    {
        public string SectionName { get; set; }
        public decimal Amount { get; set; }
    }

    public class ClassWiseFee
    {
        public string ClassName { get; set; }
        public decimal Amount { get; set; }
        public List<SectionWiseFee> Section { get; set; }
    }

    public class GetClassSectionWiseResponse
    { 
        public List<ClassWiseFee> CollectedAmount { get; set; }
    }
}
