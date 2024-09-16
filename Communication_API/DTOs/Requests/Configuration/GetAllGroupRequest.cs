namespace Communication_API.DTOs.Requests.Configuration
{
    public class GetAllGroupRequest
    {
        public int AcademicYearID { get; set; }
        public int InstituteID { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
