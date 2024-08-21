namespace HostelManagement_API.DTOs.Responses
{
    public class GetAllRoomTypesResponse
    {
        public int RoomTypeID { get; set; }
        public string RoomTypeName { get; set; }
        public bool IsActive { get; set; }
    }
}
