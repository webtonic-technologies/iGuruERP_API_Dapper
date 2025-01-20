namespace HostelManagement_API.DTOs.Requests
{
    public class SetMealPlannerRequest
    {
        public int MealPlannerID { get; set; }
        public int MealTypeID { get; set; }
        public int DayID { get; set; }
        public List<MealItems> Items { get; set; }
    }

    public class MealItems
    {
        public int MealPlannerID { get; set; }
        public string MealItem { get; set; }
    }
}
