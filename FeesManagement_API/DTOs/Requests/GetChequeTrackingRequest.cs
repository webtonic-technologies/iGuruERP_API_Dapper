using System;

namespace FeesManagement_API.DTOs.Requests
{
    public class GetChequeTrackingRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ChequeStatusID { get; set; }
    }
}
