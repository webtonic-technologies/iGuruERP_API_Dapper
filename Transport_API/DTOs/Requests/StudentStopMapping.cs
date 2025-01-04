namespace Transport_API.DTOs.Requests
{
    public class StudentStopMapping
    {
        public int StudentStopID { get; set; }
        public int StopID { get; set; }
        public int StudentID { get; set; }
        public int DeAllocatedBy { get; set; }
        public string Reason { get; set; }
        public string DeAllocationDate { get; set; }
    }
    public class EmployeeStopMapping
    {
        public int EmployeeStopID { get; set; }
        public int StopID { get; set; }
        public int EmployeeID { get; set; }
        public int DeAllocatedBy { get; set; }
        public string Reason { get; set; }
        public string DeAllocationDate { get; set; }
    }
}