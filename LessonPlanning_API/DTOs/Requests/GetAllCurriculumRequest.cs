using Microsoft.AspNetCore.Http.HttpResults;

namespace Lesson_API.DTOs.Requests
{
    public class GetAllCurriculumRequest : BaseRequest
    {
        public int InstituteID { get; set; }
    }
}
