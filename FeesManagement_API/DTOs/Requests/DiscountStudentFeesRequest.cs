namespace FeesManagement_API.DTOs.Requests
{
    public class DiscountStudentFeesRequest
    {
        public int StudentID { get; set; }
        public int FeeHeadID { get; set; }
        public int FeeGroupID { get; set; }
        public int FeeTenurityID { get; set; }
        public decimal DiscountedAmount { get; set; }
        public int InstituteID { get; set; }
        public string AcademicYearCode { get; set; }
        public int UserID { get; set; }

    }
}
