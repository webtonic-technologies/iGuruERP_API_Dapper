namespace Communication_API.DTOs.Requests.Configuration
{
    public class GetAllGroupRequest
    {
        public string AcademicYearCode { get; set; }
        public int InstituteID { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Search { get; set; }
    }
}
