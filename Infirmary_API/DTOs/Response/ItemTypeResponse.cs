namespace Infirmary_API.DTOs.Response
{
    public class ItemTypeResponse
    {
        public int ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
