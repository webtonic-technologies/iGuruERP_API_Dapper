namespace HostelManagement_API.Models
{
    public class Hostel
    {
        public int HostelID { get; set; }
        public string HostelName { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
