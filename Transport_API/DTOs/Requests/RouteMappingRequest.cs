namespace Transport_API.DTOs.Requests
{
    public class RouteMappingRequest
    {
        public int AssignRouteID { get; set; }
        public int RoutePlanID { get; set; }
        public int VehicleID { get; set; }
        public int DriverID { get; set; }
        public int TransportStaffID { get; set; }
        public bool IsActive { get; set; }
        public int StopID { get; set; } // Added StopID
        public List<int> StudentIDs { get; set; } // Added for multiple StudentIDs
        public List<int> EmployeeIDs { get; set; } // Added for multiple EmployeeIDs
    }

}
