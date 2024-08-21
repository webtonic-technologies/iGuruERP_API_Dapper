namespace FeesManagement_API.DTOs.Requests
{
    public class AddUpdateOptionalFeeRequest
    {
        public int InstituteID { get; set; }
        public List<OptionalFeeDto> OptionalFees { get; set; }
    }

    public class OptionalFeeDto
    {
        public int OptionalFeeID { get; set; }
        public string HeadName { get; set; }
        public string ShortName { get; set; }
        public decimal FeeAmount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
