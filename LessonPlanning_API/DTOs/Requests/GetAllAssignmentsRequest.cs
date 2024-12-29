using System.Runtime.InteropServices;

namespace Lesson_API.DTOs.Requests
{
    public class GetAllAssignmentsRequest : BaseRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }   
        public string SearchText { get; set; }
        public int TypeWise { get; set; }
    }
}
