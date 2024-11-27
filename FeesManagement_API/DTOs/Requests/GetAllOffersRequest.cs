namespace Configuration.DTOs.Requests
{
    public class GetAllOffersRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string AcademicYear { get; set; }
    }
}
