namespace Transport_API.DTOs.Response
{
    public class RouteDetailsResponseDTO
    {
        public int RoutePlanID { get; set; } // RoutePlanID from tblRoutePlan
        public string RouteName { get; set; } // RouteName from tblRoutePlan
        public List<RouteStopResponseDTO> Stops { get; set; } // List of stops
    }

    public class RouteStopResponseDTO
    {
        public string StopName { get; set; } // StopName from tblRouteStopMaster
        public string PickUpTime { get; set; } // PickUpTime from tblRouteStopMaster
        public string DropTime { get; set; } // DropTime from tblRouteStopMaster
        public decimal Fee { get; set; } // FeeAmount from tblRouteStopMaster
    }
}
