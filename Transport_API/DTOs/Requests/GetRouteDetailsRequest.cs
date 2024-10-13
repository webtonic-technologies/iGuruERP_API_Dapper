namespace Transport_API.DTOs.Requests
{
    public class GetRouteDetailsRequest
    {
        public int RouteID { get; set; } // This maps to RoutePlanID from tblRoutePlan
        public int InstituteID { get; set; } // This maps to InstituteID from tblRoutePlan
    }
}
