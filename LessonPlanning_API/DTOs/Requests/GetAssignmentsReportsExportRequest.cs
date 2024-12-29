using System;

namespace Lesson_API.DTOs.Requests
{
    public class GetAssignmentsReportsExportRequest : BaseRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SearchText { get; set; }
        public int TypeWise { get; set; }  // 1 for ClassWise, 2 for StudentWise
        public int ClassID { get; set; }  // Required if TypeWise = 1
        public int SectionID { get; set; }  // Required if TypeWise = 1
        public int ExportType { get; set; }  // 1 for Excel, 2 for CSV
    }
}
