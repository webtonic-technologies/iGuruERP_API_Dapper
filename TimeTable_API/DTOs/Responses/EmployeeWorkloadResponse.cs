using System.Collections.Generic;

namespace TimeTable_API.DTOs.Responses
{
    public class EmployeeWorkloadResponse
    {
        public EmployeeDetailsResponse EmployeeDetails { get; set; }
        public List<WorkloadDetailsResponse> WorkloadDetails { get; set; }
        public SessionCountResponse SessionCount { get; set; }
    }

    public class EmployeeDetailsResponse
    {
        public string EmpPhoto { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmailID { get; set; }
        public string MobileNumber { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
    }

    public class WorkloadDetailsResponse
    {
        public string Class { get; set; }
        public string Section { get; set; }
        public string Subject { get; set; }
        public int AssignedSession { get; set; }
    }

    public class SessionCountResponse
    {
        public int TotalSessions { get; set; }
        public int AssignedSessions { get; set; }
    }
}
