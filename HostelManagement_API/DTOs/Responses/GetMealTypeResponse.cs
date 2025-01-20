namespace HostelManagement_API.DTOs.Responses
{
    public class GetMealTypeResponse
    {
        public int MealTypeID { get; set; }
        public string MealType { get; set; }
        public string DayIDs { get; set; }  // Example: "1,2,3"
        public string DayTypes { get; set; }  // Example: "Mon, Tue, Wed"
        public string StartTime { get; set; }  // Example: "09:00 AM"
        public string EndTime { get; set; }  // Example: "02:00 PM"
    }
}
