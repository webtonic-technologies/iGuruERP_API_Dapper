namespace Institute_API.Models
{
    public class InstituteSMMapping
    {
        public int SM_Mapping_Id { get; set; }
        public int Institute_id { get; set; }
        public int SM_Id { get; set; }
        public string URL { get; set; } = string.Empty;
    }
}
