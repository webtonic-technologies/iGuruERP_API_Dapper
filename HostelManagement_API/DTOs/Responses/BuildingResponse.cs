namespace HostelManagement_API.DTOs.Responses
{
    public class BuildingResponse
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public int BlockID { get; set; }
        public string BlockName { get; set; }  // Added BlockName
        public bool IsActive { get; set; }
    }
}
