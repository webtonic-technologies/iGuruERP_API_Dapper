using System;

namespace FeesManagement_API.DTOs.Requests
{
    public class GetChequeTrackingBouncedRequest
    {
        public string StartDate { get; set; }    // Format "DD-MM-YYYY"
        public string EndDate { get; set; }      // Format "DD-MM-YYYY"
        public string Search { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
