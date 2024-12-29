namespace Lesson_API.DTOs.Requests
{
    public class GetChaptersRequest
    {
        public int ClassID { get; set; }
        public int SubjectID { get; set; }
        public int InstituteID { get; set; }
    }
}
