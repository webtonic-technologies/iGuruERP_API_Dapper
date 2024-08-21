namespace Student_API.DTOs
{
    public class DocumentUpdateRequest
    {
        public int StudentId { get; set; }
        public List<DocumentUpdateRequestInfo> StudentDocuments{get;set;}

        //public int Institute_id { get; set; }
    }
    public class DocumentUpdateRequestInfo
    {
        public int DocumentId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public bool IsSubmitted { get; set; }
    }
}
