namespace Infirmary_API.Models
{
    public class ItemTypeFetch
    {
        public int ItemTypeID { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
