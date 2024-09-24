namespace Transport_API.DTOs.Response
{
    public class RoutePlanResponseDTO
    {
        public int RoutePlanID { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public int NoOfStops { get; set; }
        public string PickUpTime { get; set; } = string.Empty;
        public string DropTime { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;

        // Add RouteStops back to the DTO
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


    //public class RoutePlanResponseDTO
    //{
    //    public int RoutePlanID { get; set; }
    //    public string RouteName { get; set; } = string.Empty;
    //    public int VehicleID { get; set; }
    //    public string VehicleName { get; set; } = string.Empty;
    //    public int InstituteID { get; set; }
    //    public int NoOfStops { get; set; }// count of rout stops
    //    public bool IsActive { get; set; }
    //    public string EmployeeName { get; set; } = string.Empty;
    //    public List<RouteStopResponse>? RouteStops { get; set; }
    //}
    //public class RouteStopResponse
    //{
    //    public int StopID { get; set; }
    //    public int RoutePlanID { get; set; }
    //    public string StopName { get; set; } = string.Empty;
    //    public string PickUpTime { get; set; } = string.Empty;
    //    public string DropTime { get; set; } = string.Empty;
    //    public decimal FeeAmount { get; set; }
    //}

}
