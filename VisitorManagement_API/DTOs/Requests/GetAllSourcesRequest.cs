namespace VisitorManagement_API.DTOs.Requests
{
    public class GetAllSourcesRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteID { get; set; }
    }
}
