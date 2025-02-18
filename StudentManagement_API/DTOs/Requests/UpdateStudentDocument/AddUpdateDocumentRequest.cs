namespace StudentManagement_API.DTOs.Requests
{
    public class AddUpdateDocumentRequest
    {
        public List<string> DocumentNames { get; set; }
        public int InstituteID { get; set; }
    }
}
