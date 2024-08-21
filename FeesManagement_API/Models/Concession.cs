namespace Configuration.Models
{
    public class Concession
    {
        public int ConcessionID { get; set; }
        public string ConcessionName { get; set; }
        public int InstituteID { get; set; }
        public int FeeHeadID { get; set; }
        public decimal ConcessionAmount { get; set; }
    }
}
