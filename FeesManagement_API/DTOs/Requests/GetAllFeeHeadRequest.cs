namespace FeesManagement_API.DTOs.Requests
{
    public class GetAllFeeHeadRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteID { get; set; } // Add InstituteID to the request
    }
}
