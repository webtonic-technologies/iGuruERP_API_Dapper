namespace Transport_API.DTOs.Requests
{
    public class GetStudentsForRouteMappingRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string Search { get; set; } // For search functionality, if needed
    }
}
