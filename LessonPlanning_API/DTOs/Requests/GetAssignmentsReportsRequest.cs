using System;

namespace Lesson_API.DTOs.Requests
{
    public class GetAssignmentsReportsRequest : BaseRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SearchText { get; set; }
        public int TypeWise { get; set; }
        public int ClassID { get; set; } // For TypeWise == 1
        public int SectionID { get; set; } // For TypeWise == 1
    }
}
