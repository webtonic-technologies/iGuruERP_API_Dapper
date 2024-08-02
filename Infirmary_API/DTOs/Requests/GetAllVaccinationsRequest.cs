namespace Infirmary_API.DTOs.Requests
{
    public class GetAllVaccinationsRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
