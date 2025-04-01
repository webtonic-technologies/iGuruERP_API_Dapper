public class StudentFeeData
{
    public int StudentID { get; set; }
    public string AdmissionNo { get; set; }
    public string StudentName { get; set; }
    public string RollNo { get; set; }
    public string ClassName { get; set; }
    public string SectionName { get; set; }
    public string ConcessionGroup { get; set; }
    public decimal LateFee { get; set; }
    public int FeeHeadID { get; set; }
    public string FeeHead { get; set; }
    public string FeeType { get; set; }
    public decimal FeeAmount { get; set; }
    public int FeeGroupID { get; set; }        // New property for FeeGroupID
    public int FeeTenurityID { get; set; }       // New property for FeeTenurityID
}
