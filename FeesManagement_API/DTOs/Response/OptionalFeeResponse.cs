namespace FeesManagement_API.DTOs.Response
{
    public class OptionalFeeResponse
    {
        public int OptionalFeeID { get; set; }
        public string HeadName { get; set; }
        public string ShortName { get; set; }
        public decimal FeeAmount { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
