namespace HostelManagement_API.Models
{
    public class Building
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public int BlockID { get; set; }
        public bool IsActive { get; set; }
    }
}
