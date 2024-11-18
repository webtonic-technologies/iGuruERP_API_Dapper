namespace Transport_API.DTOs.Requests
{
    public class GetEmployeesForRouteMappingRequest
    {
        public int InstituteID { get; set; }
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
        public string Search { get; set; } // Optional, for search functionality
    }
}
