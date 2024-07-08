namespace Institute_API.DTOs
{
    public class GetListRequest
    {
        public int Institute_id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchText { get; set; } = string.Empty;
        public string SortDirection { get; set; } = string.Empty;
    }
}
