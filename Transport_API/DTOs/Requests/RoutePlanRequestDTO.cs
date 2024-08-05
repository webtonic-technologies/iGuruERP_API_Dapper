namespace Transport_API.DTOs.Requests
{
    public class RoutePlanRequestDTO
    {
        public int RoutePlanID { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int VehicleID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<RouteStop>? RouteStops { get; set; }
    }
    public class RouteStop
    {
        public int StopID { get; set; }
        public int RoutePlanID { get; set; }
        public string StopName { get; set; } = string.Empty;
        public string PickUpTime { get; set; } = string.Empty;
        public string DropTime { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }
    }
}
