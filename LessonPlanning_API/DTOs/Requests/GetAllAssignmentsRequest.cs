using System.Runtime.InteropServices;

namespace Lesson_API.DTOs.Requests
{
    public class GetAllAssignmentsRequest : BaseRequest
    {
        public int InstituteID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }   
        public int AssignmentTypeID {  get; set; }
    }
}
