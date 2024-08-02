namespace HostelManagement_API.DTOs.Responses
{
    public class FloorResponse
    {
        public int FloorID { get; set; }
        public string FloorName { get; set; }
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }  // Added BuildingName
        public string BlockName { get; set; }     // Added BlockName
        public bool IsActive { get; set; }
    }
}
