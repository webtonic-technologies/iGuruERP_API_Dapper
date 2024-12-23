namespace Infirmary_API.DTOs.Requests
{
    public class GetAllInfirmaryVisitsRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int VisitorTypeId {  get; set; }
        public string StartDate { get; set; } // Changed to string
        public string EndDate { get; set; }   // Changed to string
        public string SearchText { get; set; } = string.Empty; // can be entry id or student/employee name
    }
}
