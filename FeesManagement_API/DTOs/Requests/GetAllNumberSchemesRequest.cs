namespace FeesManagement_API.DTOs.Requests
{
    public class GetAllNumberSchemesRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
