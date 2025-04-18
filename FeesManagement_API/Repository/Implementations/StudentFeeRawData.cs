﻿namespace FeesManagement_API.Repository.Implementations
{
    public class StudentFeeRawData
    {
        public int StudentID { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string ConcessionGroup { get; set; }
        public int FeeHeadID { get; set; }
        public string FeeHead { get; set; }
        public int FeeGroupID { get; set; }
        public int FeeTenurityID { get; set; }
        // This field determines the fee type (for example, "Tuition Fee (Single)", "Tuition Fee (M Term 1)", etc.)
        public string FeeType { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal LateFee { get; set; }
    }
}
