
namespace Institute_API.Models
{
    public class InstituteHouse
    {
        public int Institute_house_id { get; set; }
        public int Institute_id { get; set; }
        public string HouseName { get; set; } = string.Empty;
        public string HouseColor { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
