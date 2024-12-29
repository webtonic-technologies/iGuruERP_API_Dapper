namespace Lesson_API.DTOs.Requests
{
    public class GetAllHomeworkExportRequest
    {
        public int InstituteID { get; set; } // Institute ID for filtering the homework data
        public string StartDate { get; set; } // Start date for the homework filter (Format 'DD-MM-YYYY')
        public string EndDate { get; set; }   // End date for the homework filter (Format 'DD-MM-YYYY')
        public string SearchTerm { get; set; } // Term to search for HomeworkName, SubjectName, or HomeworkType
        public int ExportType { get; set; }    // 1 for Excel, 2 for CSV export type
    }
}
