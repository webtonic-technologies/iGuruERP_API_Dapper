namespace HostelManagement_API.DTOs.Requests
{
    public class AddUpdateBlockRequest
    {
        public int? BlockID { get; set; }
        public string BlockName { get; set; }
        public int InstituteID { get; set; }
    }
}
