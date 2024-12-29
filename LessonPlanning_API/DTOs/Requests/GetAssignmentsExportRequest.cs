namespace Lesson_API.DTOs.Requests
{
    public class GetAssignmentsExportRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Format: DD-MM-YYYY
        public string EndDate { get; set; }    // Format: DD-MM-YYYY
        public string SearchText { get; set; }
        public int TypeWise { get; set; }      // 1 for Classwise, 2 for Studentwise
        public int ExportType { get; set; }    // 1 for Excel, 2 for CSV
    }
}
