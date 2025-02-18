namespace StudentManagement_API.DTOs.Requests
{
    public class GetDocumentManagerRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
