namespace Infirmary_API.DTOs.Requests
{
    public class GetAllInfirmaryVisitsRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int VisitorTypeId {  get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SearchText { get; set; } = string.Empty; // can be entry id or student/employee name
    }
}
