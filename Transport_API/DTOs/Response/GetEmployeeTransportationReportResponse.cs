namespace Transport_API.DTOs.Response
{
    public class GetEmployeeTransportationReportResponse
    {
        public int VehicleID { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }
        public string CoordinatorName { get; set; }
        public string DriverName { get; set; }
        public string DriverNumber { get; set; }
        public int TotalCount { get; set; }
        public List<EmployeeDetails> Employees { get; set; }
    }

    public class EmployeeVehicleDetails
    {
        public int VehicleID { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleType { get; set; }
        public string CoordinatorName { get; set; }
        public string DriverName { get; set; }
        public string DriverNumber { get; set; }
    }

    public class EmployeeDetails
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string MobileNumber { get; set; }
        public string StopName { get; set; }
    }
}
