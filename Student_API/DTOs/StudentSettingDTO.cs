namespace Student_API.DTOs
{
    public class StudentSettingDTO
    {
        public int settingId { get; set; }
        public string DbColumnName { get; set; }
        public string DisplayName { get; set; }
        public string AliaseName { get; set; }
        public int categoryId { get; set; }
        public bool IsActive { get; set; }
        public int Institute_id { get; set; }
    }

}
