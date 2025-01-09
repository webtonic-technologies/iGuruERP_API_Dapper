namespace Transport_API.DTOs.Requests
{
    public class GetTransportReportRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
