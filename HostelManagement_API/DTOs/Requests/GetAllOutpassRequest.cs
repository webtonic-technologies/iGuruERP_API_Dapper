namespace HostelManagement_API.DTOs.Requests
{
    public class GetAllOutpassRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }
}
