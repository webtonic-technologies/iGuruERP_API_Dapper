namespace HostelManagement_API.DTOs.Responses
{
    public class RoomTypeResponse
    {
        public int RoomTypeID { get; set; }
        public string RoomTypeName { get; set; }  // This should map to RoomType column
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
