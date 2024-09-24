public class RouteMapping
{
    public int AssignRouteID { get; set; }
    public int RoutePlanID { get; set; }
    public int VehicleID { get; set; }
    public int DriverID { get; set; }
    public int TransportStaffID { get; set; }
    public bool IsActive { get; set; }
    public int StopID { get; set; } // Add StopID
    public List<int> StudentIDs { get; set; } = new List<int>(); // Add StudentIDs
    public List<int> EmployeeIDs { get; set; } = new List<int>(); // Add EmployeeIDs
}

