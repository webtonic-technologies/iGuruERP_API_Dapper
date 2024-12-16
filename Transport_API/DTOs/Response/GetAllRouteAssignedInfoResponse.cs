namespace Transport_API.DTOs.Responses
{
    public class GetAllRouteAssignedInfoResponse
    {
        public int TotalStudentCount { get; set; } // Total student count across all stops
        public int TotalEmployeeCount { get; set; } // Total employee count across all stops
        public RouteDetailsResponseDTO1 RouteDetails { get; set; } // Route details with stops
    }

    public class RouteDetailsResponseDTO1
    {
        public int RoutePlanID { get; set; } // RoutePlanID from tblRoutePlan
        public string RouteName { get; set; } // RouteName from tblRoutePlan
        public List<RouteStopResponseDTO1> Stops { get; set; } // List of stops
    }

    public class RouteStopResponseDTO1
    {
        public string StopName { get; set; } // StopName from tblRouteStopMaster
        public string PickUpTime { get; set; } // PickUpTime from tblRouteStopMaster
        public string DropTime { get; set; } // DropTime from tblRouteStopMaster
        public decimal Fee { get; set; } // FeeAmount from tblRouteStopMaster
        public int StudentCount { get; set; } // Count of students at the stop
        public int EmployeeCount { get; set; } // Count of employees at the stop
    }
}
