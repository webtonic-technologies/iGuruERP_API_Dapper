namespace Configuration_API.Models
{
    public class DaySetup
    {
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public string DayIDs { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
