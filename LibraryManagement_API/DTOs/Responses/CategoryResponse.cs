namespace LibraryManagement_API.DTOs.Responses
{
    public class CategoryResponse
    {
        public int CategoryID { get; set; }
        public int InstituteID { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}
