namespace ProductUpdates_API.DTOs.Response
{
    public class PostResponse
    {
        public int PostID { get; set; }
        public string Heading { get; set; }
        public int CreatedBy { get; set; }
        public DateTime PostDate { get; set; }
        public int ModuleID { get; set; }
        public string Description { get; set; }
        public string IsActive { get; set; }
    }
}
