namespace HostelManagement_API.DTOs.Responses
{
    public class GetBedInformationResponse
    {
        public int RoomBedID { get; set; }
        public string RoomBedName { get; set; }
        public string BedPosition { get; set; }
        public string Availability { get; set; } // Occupied/Available
    }
}
