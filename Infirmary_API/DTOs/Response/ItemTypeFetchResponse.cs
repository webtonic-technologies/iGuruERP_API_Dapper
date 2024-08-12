namespace Infirmary_API.DTOs.Response
{
    public class ItemTypeFetchResponse
    {
        public int ItemTypeID { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
