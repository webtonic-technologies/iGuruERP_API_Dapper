namespace Transport_API.DTOs.Responses
{
    public class GetDeAllocationReportExportExcelResponse
    {
        public int StudentID { get; set; }
        public string AdmissionNumber { get; set; }
        public string StudentName { get; set; }
        public string ClassSection { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Mobile { get; set; }
        public string RouteName { get; set; }
        public string StopName { get; set; }
        public string VehicleNumber { get; set; }
        public string DeAllocatedBy { get; set; }
        public string Reason { get; set; }
        public string DeAllocationDate { get; set; }  // Date as string
    }
}
