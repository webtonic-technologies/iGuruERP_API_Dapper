using System;

namespace FeesManagement_API.DTOs.Requests
{
    public class GetChequeTrackingRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; } 
        public string Search { get; set; }
        public int PageNumber { get; set; } 
        public int PageSize { get; set; }

    }
}
