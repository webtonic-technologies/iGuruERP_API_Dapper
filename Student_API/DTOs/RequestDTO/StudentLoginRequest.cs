namespace Student_API.DTOs.RequestDTO
{
    public class StudentLoginRequest
    {
        public int InstituteId { set; get; }
        public int ClassId { set; get; }
        public int SectionId { set; get; }
        public string SearchText { set; get; } = string.Empty;
        public int PageNumber { set; get; }
        public int PageSize { set; get; }
    }
}
