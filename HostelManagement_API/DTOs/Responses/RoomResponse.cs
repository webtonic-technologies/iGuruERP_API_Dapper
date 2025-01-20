namespace HostelManagement_API.DTOs.Responses
{
    public class RoomResponse
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public int RoomTypeID { get; set; }
        public string RoomTypeName { get; set; } // From tblRoomType
        public int HostelID { get; set; }
        public string HostelName { get; set; } // From tblHostel
        public int FloorID { get; set; }
        public string FloorName { get; set; } // From tblBuildingFloors
        public bool IsActive { get; set; }
    }
}
