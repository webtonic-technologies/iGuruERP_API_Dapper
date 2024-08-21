namespace HostelManagement_API.Models
{
    public class Block
    {
        public int BlockID { get; set; }
        public string BlockName { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
