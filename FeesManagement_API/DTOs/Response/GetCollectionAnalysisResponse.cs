using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class DayWiseCollection
    {
        public string Date { get; set; }  // Formatted as "MMM dd" e.g., "Apr 13"
        public decimal Amount { get; set; }
    }

    public class GetCollectionAnalysisResponse
    {
        public List<DayWiseCollection> AmountCollected { get; set; }
    }
}
