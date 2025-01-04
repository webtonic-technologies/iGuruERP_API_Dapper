namespace Transport_API.DTOs.Requests
{
    public class GetTransportationPendingFeeReportRequest
    {
        public int InstituteID { get; set; }
        public int RoutePlanID { get; set; }
    }
}
