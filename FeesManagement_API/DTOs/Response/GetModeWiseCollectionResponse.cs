using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class ModeWiseCollection
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }

    public class GetModeWiseCollectionResponse
    {
        public List<ModeWiseCollection> Collections { get; set; }
    }
}
