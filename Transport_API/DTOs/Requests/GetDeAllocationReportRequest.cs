namespace Transport_API.DTOs.Requests
{
    public class GetDeAllocationReportRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int UserTypeID { get; set; }  // 1 for Employee, 2 for Student
    }
}
