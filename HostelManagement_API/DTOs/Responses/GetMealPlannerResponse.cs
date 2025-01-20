namespace HostelManagement_API.DTOs.Responses
{
    public class GetMealPlannerResponse
    {
        public int DayID { get; set; }
        public string DayType { get; set; }
        public List<MealPlanner> MealPlanner { get; set; }
    }

    public class MealPlanner
    {
        public int MealTypeID { get; set; }
        public string MealType { get; set; }
        public string MealTime { get; set; }
        public List<MenuItems> MealMenu { get; set; }
    }

    public class MenuItems
    {
        public string MenuItem { get; set; }
    }
}
