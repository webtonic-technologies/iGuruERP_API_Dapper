namespace FeesManagement_API.Models
{
    public class FeeHead
    {
        public int FeeHeadID { get; set; }
        public string FeeHeadName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public int RegTypeID { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }
}
