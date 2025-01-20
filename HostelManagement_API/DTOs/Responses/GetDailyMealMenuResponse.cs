namespace HostelManagement_API.DTOs.Responses
{
    public class MealItem
    {
        public string MealItemName { get; set; } 
    }

    public class GetDailyMealMenuResponse
    {
        public string MealType { get; set; }
        public string MealTime { get; set; }
        public string StartTime { get; set; }  // Format should be 09:00 AM
        public string EndTime { get; set; }    // Format should be 02:00 PM
        public List<MealItem> MealItems { get; set; }
    }
}
