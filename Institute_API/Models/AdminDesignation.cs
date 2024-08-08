namespace Institute_API.Models
{
    public class AdminDesignation
    {
        public int Designation_id { get; set; }
        public int Institute_id { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int Department_id { get; set; }
        public bool IsDeleted {  get; set; }
    }
    public class AdminDesignationResponse
    {
        public int Designation_id { get; set; }
        public int Institute_id { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int Department_id { get; set; }
        public string Department_name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
