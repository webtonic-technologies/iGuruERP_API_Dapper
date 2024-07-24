namespace HostelManagement_API.Models
{
    public class RoomType
    {
        public int RoomTypeID { get; set; }
        public string RoomTypeName { get; set; }
        public int InstituteID { get; set; }  // Added InstituteID
        public bool IsActive { get; set; }
    }
}
