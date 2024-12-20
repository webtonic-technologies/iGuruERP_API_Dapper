namespace VisitorManagement_API.DTOs.Requests
{
    public class GetAllAppointmentsRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteId { get; set; }
        public string StartDate { get; set; }  // Changed to string
        public string EndDate { get; set; }  // Changed to string

    }
}
