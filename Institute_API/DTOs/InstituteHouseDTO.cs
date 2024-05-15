namespace Institute_API.DTOs
{
    public class InstituteHouseDTO
    {
        public int Institute_id { get; set; }
        public List<InstituteHouses>? InstituteHouses { get; set; }
    }
    public class InstituteHouses
    {
        public int Institute_house_id { get; set; }
        public int Institute_id { get; set; }
        public string HouseName { get; set; } = string.Empty;
        public string HouseColor { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime? en_date {  get; set; }
    }
}
