namespace HostelManagement_API.DTOs.Requests
{
    public class GetAllRoomsRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
