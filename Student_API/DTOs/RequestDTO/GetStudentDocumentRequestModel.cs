namespace Student_API.DTOs.RequestDTO
{
    public class GetStudentDocumentRequestModel : PagedListModel
    {
        public int classId { get; set; }
        public int sectionId { get; set; }
    }
}
