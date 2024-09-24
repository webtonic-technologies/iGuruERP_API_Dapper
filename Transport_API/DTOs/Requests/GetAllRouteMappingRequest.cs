namespace Transport_API.DTOs.Requests
{
    public class GetAllRouteMappingRequest
    {
        public int InstituteID { get; set; } // Added InstituteID property
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
