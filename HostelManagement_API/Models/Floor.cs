namespace HostelManagement_API.Models
{
    public class Floor
    {
        public int FloorID { get; set; }
        public string FloorName { get; set; }
        public int BuildingID { get; set; }
        public int InstituteID { get; set; }  // Added InstituteID
        public bool IsActive { get; set; }
    }
}
