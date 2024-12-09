using Microsoft.AspNetCore.Http.HttpResults;

namespace Lesson_API.DTOs.Requests
{
    public class GetAllCurriculumRequest : BaseRequest
    {
        public string AcademicYearID { get; set; }
        public int ClassID { get; set; }
        public int InstituteID { get; set; }
    }
}
