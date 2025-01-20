namespace HostelManagement_API.DTOs.Requests
{
    public class GetBedInformationRequest
    {
        public int InstituteID { get; set; }
        public int HostelID { get; set; }
        public int RoomID { get; set; }
    }
}
