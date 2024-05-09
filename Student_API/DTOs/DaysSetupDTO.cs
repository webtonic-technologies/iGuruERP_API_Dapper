namespace Student_API.DTOs
{
    public class DaysSetupDTO
    {
        public int DaysSetupId { get; set; }
        public string PlanName { get; set; }
        public string WorkingDays { get; set; }
        public List<int> TimetableGroupIds { get; set; }
    }
}
