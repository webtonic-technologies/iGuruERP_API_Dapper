namespace FeesManagement_API.DTOs.Responses
{
    public class StudentFeeResponse1
    {
        public int StudentID { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string ConcessionGroup { get; set; }
        public int FeeGroupID { get; set; }
        public int FeeTenurityID { get; set; }
        public decimal TotalLateFee { get; set; }
        public decimal TotalFeeAmount { get; set; }
        public List<StudentFeeDetail1> FeeDetails { get; set; }
    }

    public class StudentFeeDetail1
    {
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; }
        public string TenureType { get; set; }
        public decimal Amount { get; set; }
        public decimal LateFee { get; set; }
    }
}
