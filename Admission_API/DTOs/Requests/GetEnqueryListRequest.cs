namespace Admission_API.DTOs.Requests
{
    public class GetEnqueryListRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteID { get; set; }
    }
}
 