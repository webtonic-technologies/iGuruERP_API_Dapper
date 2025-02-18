namespace StudentManagement_API.DTOs.Requests
{
    public class GetDocumentsRequest
    {
        public int InstituteID { get; set; }
        public string Search { get; set; }
    }
}
