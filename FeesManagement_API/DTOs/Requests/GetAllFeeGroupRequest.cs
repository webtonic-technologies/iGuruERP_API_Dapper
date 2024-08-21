namespace FeesManagement_API.DTOs.Requests
{
    public class GetAllFeeGroupRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int FeeHeadID { get; set; }
        public int InstituteID { get; set; }
    }
}
