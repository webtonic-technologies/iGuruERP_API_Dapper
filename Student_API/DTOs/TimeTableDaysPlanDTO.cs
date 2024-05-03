using System.Text.Json.Serialization;

namespace Student_API.DTOs
{
    public class TimeTableDaysPlanDTO
    {
        public int DaysSetupId { get; set; }
        public string PlanName { get; set; }
        [JsonIgnore]
        public string TimetableGroupName { get; set; }
        public List<string> TimetableGroupNames { get; set; }
    }

}
