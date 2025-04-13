using System.Collections.Generic;

namespace FeesManagement_API.DTOs.Responses
{
    public class TypeWiseCollection
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }

    public class GetTypeWiseCollectionResponse
    {
        public List<TypeWiseCollection> Collections { get; set; }
    }
}
