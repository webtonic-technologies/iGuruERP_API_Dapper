using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class DayWiseFee
    {
        public string Day { get; set; } // E.g., "Apr 13"
        public decimal Amount { get; set; }
    }

    public class GetDayWiseFeesResponse
    {
        public List<DayWiseFee> DayWiseFees { get; set; }
    }
}
