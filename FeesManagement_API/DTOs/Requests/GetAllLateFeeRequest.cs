namespace FeesManagement_API.DTOs.Requests
{
    public class GetAllLateFeeRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        //public string Search { get; set; }
    }
}
