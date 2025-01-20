namespace HostelManagement_API.DTOs.Requests
{
    public class AddMealTypeRequest
    {
        public int MealTypeID { get; set; }
        public string MealType { get; set; }
        public string DayIDs { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<MealDocument> MealDocuments { get; set; }
    }

    public class MealDocument
    {
        public int MealTypeDocID { get; set; }
        public int MealTypeID { get; set; }
        public byte[] DocFile { get; set; }
    }
}
