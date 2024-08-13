using System.Collections.Generic;

namespace LibraryManagement_API.DTOs.Requests
{
    public class AddUpdateGenreRequest
    {
        public List<GenreDTO> Genres { get; set; }
    }

    public class GenreDTO
    {
        public int GenreID { get; set; }
        public int InstituteID { get; set; }
        public string GenreName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
