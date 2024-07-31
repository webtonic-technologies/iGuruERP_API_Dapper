namespace ProductUpdates_API.DTOs.Requests
{
    public class CreatePostRequest
    {
        public string Heading { get; set; }
        public int CreatedBy { get; set; }
        public DateTime PostDate { get; set; }
        public int ModuleID { get; set; }
        public string Description { get; set; }
    }
}
