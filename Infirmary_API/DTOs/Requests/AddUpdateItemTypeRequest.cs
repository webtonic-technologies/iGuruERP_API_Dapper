namespace Infirmary_API.DTOs.Requests
{
    public class AddUpdateItemTypeRequest
    {
        public int ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }
}
