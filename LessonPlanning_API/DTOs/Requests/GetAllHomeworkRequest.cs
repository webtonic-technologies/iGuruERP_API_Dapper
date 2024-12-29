namespace Lesson_API.DTOs.Requests
{
    public class GetAllHomeworkRequest : BaseRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; } // Format 'DD-MM-YYYY'
        public string EndDate { get; set; }   // Format 'DD-MM-YYYY'
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }      // For pagination
        public int PageSize { get; set; }        // For pagination
    }
}
