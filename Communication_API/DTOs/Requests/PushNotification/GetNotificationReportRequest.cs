namespace Communication_API.DTOs.Requests.PushNotification
{
    public class GetNotificationReportRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int UserTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
