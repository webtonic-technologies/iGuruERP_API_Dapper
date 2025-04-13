using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class HeadWiseCollected
    {
        public string FeeHead { get; set; }
        public decimal Percentage { get; set; }
    }

    public class GetHeadWiseCollectedAmountResponse
    {
        public decimal TotalAmount { get; set; }
        public List<HeadWiseCollected> HeadWise { get; set; }
    }
}
