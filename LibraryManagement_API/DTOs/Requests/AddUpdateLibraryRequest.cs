using System.Collections.Generic;

namespace LibraryManagement_API.DTOs.Requests
{
    public class AddUpdateLibraryRequest
    {
        public List<LibraryDTO> Libraries { get; set; }
    }

    public class LibraryDTO
    {
        public int LibraryID { get; set; }
        public int InstituteID { get; set; }
        public string LibraryName { get; set; }
        public string ShortName { get; set; }
        public int LibraryInchargeID { get; set; }
        public bool IsActive { get; set; }
    }
}
