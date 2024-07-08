﻿namespace Transport_API.DTOs.Response
{
    public class RoutePlanResponse
    {
        public int RoutePlanId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public int InstituteId { get; set; }
    }
    public class RoutePlanResponseDTO
    {
        public int RoutePlanID { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int VehicleID { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<RouteStopResponse>? RouteStops { get; set; }
    }
    public class RouteStopResponse
    {
        public int StopID { get; set; }
        public int RoutePlanID { get; set; }
        public string StopName { get; set; } = string.Empty;
        public string PickUpTime { get; set; } = string.Empty;
        public string DropTime { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }
    }
}
