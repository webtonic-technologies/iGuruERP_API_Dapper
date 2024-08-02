using System.Collections.Generic;

namespace HostelManagement_API.DTOs.Requests
{
    public class AddUpdateRoomTypeRequest
    {
        public int? RoomTypeID { get; set; }
        public string RoomTypeName { get; set; }
        public int InstituteID { get; set; }  // Added InstituteID
        public List<int> RoomFacilityIDs { get; set; }  // List of RoomFacilityIDs
        public List<int> BuildingFacilityIDs { get; set; }  // List of BuildingFacilityIDs
        public List<int> OtherFacilityIDs { get; set; }  // List of OtherFacilityIDs
    }
}
