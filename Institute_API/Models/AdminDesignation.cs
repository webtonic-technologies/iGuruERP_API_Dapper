namespace Institute_API.Models
{
    public class AdminDesignation
    {
        public int Designation_id { get; set; }
        public int Institute_id { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int Department_id { get; set; }
    }
}
