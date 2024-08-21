namespace FeesManagement_API.DTOs.Response
{
    public class FeeHeadResponse
    {
        public int FeeHeadID { get; set; }
        public string FeeHeadName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public int RegTypeID { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }
}
