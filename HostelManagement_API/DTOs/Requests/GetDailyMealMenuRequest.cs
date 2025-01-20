namespace HostelManagement_API.DTOs.Requests
{
    public class GetDailyMealMenuRequest
    {
        public int InstituteID { get; set; }
        public int MealTypeID { get; set; } 
        public string Date { get; set; }
    }
}
