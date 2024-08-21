using System.Collections.Generic;

namespace Infirmary_API.DTOs.Requests
{
    public class AddUpdateItemTypeRequest
    {
        public List<ItemTypeDTO> ItemTypes { get; set; }
    }

    public class ItemTypeDTO
    {
        public int ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public string Description { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
