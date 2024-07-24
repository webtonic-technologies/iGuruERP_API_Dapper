using System.Collections.Generic;

namespace HostelManagement_API.DTOs.Requests
{
    public class AddUpdateRoomRequest
    {
        public int? RoomID { get; set; }
        public string RoomName { get; set; }
        public int HostelID { get; set; }
        public int FloorID { get; set; }
        public int RoomTypeID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<RoomBedRequest> RoomBeds { get; set; }
    }

    public class RoomBedRequest
    {
        public int? RoomBedID { get; set; }
        public string RoomBedName { get; set; }
        public string BedPosition { get; set; }
    }
}
