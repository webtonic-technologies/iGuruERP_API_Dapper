namespace HostelManagement_API.DTOs.Requests
{
    public class GetAllBuildingsRequest
    { 
        public int InstituteID { get; set; }  // Added InstituteID
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
