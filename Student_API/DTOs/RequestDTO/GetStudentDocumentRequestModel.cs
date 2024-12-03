namespace Student_API.DTOs.RequestDTO
{
    public class GetStudentDocumentRequestModel : PagedListModel
    {
        public int classId { get; set; } = 0;
        public int sectionId { get; set; } = 0;
        public int Institute_id { get; set; }
        public string searchQuery { get; set; }

    }

    public class ExportStudentDocumentRequestModel
    {
        public int classId { get; set; } = 0;
        public int sectionId { get; set; } = 0;
        public int Institute_id { get; set; }
        public int exportFormat { get; set; }
    }
}
