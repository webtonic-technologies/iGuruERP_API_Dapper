namespace StudentManagement_API.DTOs.Responses
{
    public class GetClassPromotionResponse
    {
        public int ClassID { get; set; }
        public string Class { get; set; }
        public int? PromotedClassID { get; set; }
        public string PromotedClass { get; set; }
    }
}
