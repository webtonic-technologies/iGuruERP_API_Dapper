using System.Collections.Generic;

namespace LibraryManagement_API.DTOs.Requests
{
    public class AddUpdateAuthorsRequest
    {
        public List<AuthorRequest> Authors { get; set; }
    }

    public class AuthorRequest
    {
        public int AuthorID { get; set; }
        public int InstituteID { get; set; }
        public string AuthorName { get; set; }
        public bool IsActive { get; set; }
    }
}
