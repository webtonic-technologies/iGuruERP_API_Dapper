namespace HostelManagement_API.DTOs.Requests
{
    public class GetAllFloorsRequest
    {
        public int InstituteID { get; set; }  // Added InstituteID
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
