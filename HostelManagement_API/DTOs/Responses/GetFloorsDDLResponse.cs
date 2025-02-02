namespace HostelManagement_API.DTOs.Responses
{
    public class GetFloorsDDLResponse
    {
        public int FloorID { get; set; }
        public string FloorName { get; set; }
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
    }
}
