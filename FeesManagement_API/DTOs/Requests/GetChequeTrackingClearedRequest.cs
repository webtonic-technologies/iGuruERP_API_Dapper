namespace FeesManagement_API.DTOs.Requests
{
    public class GetChequeTrackingClearedRequest
    {
        // Date range (in "DD-MM-YYYY" format)
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        // Optional search string
        public string Search { get; set; }

        // Paging parameters
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
