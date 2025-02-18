namespace StudentManagement_API.DTOs.Responses
{
    public class GetDocumentsResponse
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        // This property is expected to be formatted as "dd-MM-yyyy at hh:mm tt"
        public string CreatedDateTime { get; set; }
    }
}
