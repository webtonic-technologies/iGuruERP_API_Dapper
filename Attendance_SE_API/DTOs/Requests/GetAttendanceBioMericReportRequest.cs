namespace Attendance_SE_API.DTOs.Requests
{
    public class GetAttendanceBioMericReportRequest
    {
        public int InstituteID { get; set; }
        public string StartDate { get; set; }  // Date in 'DD-MM-YYYY' format
        public string EndDate { get; set; }    // Date in 'DD-MM-YYYY' format
        public int PageNumber { get; set; }    // Pagination: page number
        public int PageSize { get; set; }      // Pagination: page size
        public string SearchTerm { get; set; } // Optional search term
    }

}
