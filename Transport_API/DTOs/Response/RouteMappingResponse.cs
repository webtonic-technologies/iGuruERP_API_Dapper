namespace Transport_API.DTOs.Response
{
    //public class RouteMappingResponse
    //{
    //    public int RouteMappingId { get; set; }
    //    public string RouteName { get; set; } = string.Empty;
    //    public int VehicleId { get; set; }
    //    public string VehicleNumber { get; set; } = string.Empty;
    //    public int EmployeeId { get; set; }
    //    public string DriveName { get; set; } = string.Empty;
    //    public int TransportStaffID {  get; set; }
    //    public string TransportStaffName { get; set; } = string.Empty;
    //    public int TotalStudents {  get; set; }
    //    public int TotalEmployee {  get; set; }
    //}
    public class RouteMappingResponse
    {
        public int RouteMappingId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string DriveName { get; set; } = string.Empty; // Should be 'DriverName'
        public string TransportStaffName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public int TotalEmployee { get; set; } // Should be 'TotalEmployees'
        public int Availability { get; set; } // Add this property if it doesn't exist
    }

    public class StudentStopMappingResponse
    {
        public int StudentStopID { get; set; }
        public int StopID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber {  get; set; } = string.Empty;
    }

    public class EmployeeStopMappingResponse
    {
        public int EmployeeStopID { get; set; }
        public int StopID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } // Changed from int to string
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
    }


    //public class EmployeeStopMappingResponse
    //{
    //    public int EmployeeStopID { get; set; }
    //    public int StopID { get; set; }
    //    public int EmployeeID { get; set; }
    //    public int EmployeeName {  get; set; }
    //    public int DesignationId {  get; set; }
    //    public string DesignationName { get; set; } = string.Empty;
    //}
}
