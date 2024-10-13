namespace Transport_API.DTOs.Requests
{
    public class StudentsReportRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int InstituteID { get; set; }
        public string Status { get; set; }  // All, Allocated, Non-Allocated
    }
}
