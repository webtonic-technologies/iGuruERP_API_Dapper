namespace StudentManagement_API.DTOs.Requests
{
    public class GetClassPromotionHistoryRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
