namespace Transport_API.DTOs.Requests
{
    public class GetReportsRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? RoutePlanID { get; set; }  // Nullable to handle cases where it's not required
        public int? InstituteID { get; set; }  // Nullable to handle cases where it's not required
        public string Search { get; set; }

    }
}
