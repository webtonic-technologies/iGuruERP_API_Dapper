namespace HostelManagement_API.DTOs.Requests
{
    public class AddUpdateFloorRequest
    {
        public int? FloorID { get; set; }
        public string FloorName { get; set; }
        public int BuildingID { get; set; }
        public int InstituteID { get; set; }  // Added InstituteID
    }
}
