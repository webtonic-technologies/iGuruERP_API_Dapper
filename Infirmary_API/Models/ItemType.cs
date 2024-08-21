namespace Infirmary_API.Models
{
    public class ItemType
    {
        public int ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public string Description { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
