namespace Configuration_API.DTOs.Requests
{
    public class AddUpdateDaySetupRequest
    {
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public string DayIDs { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
