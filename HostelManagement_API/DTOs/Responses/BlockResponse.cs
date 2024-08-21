namespace HostelManagement_API.DTOs.Responses
{
    public class BlockResponse
    {
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
