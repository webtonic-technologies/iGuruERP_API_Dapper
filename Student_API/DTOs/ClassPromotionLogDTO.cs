namespace Student_API.DTOs
{
    public class ClassPromotionLogDTO
    {
        public int LogId { get; set; }
        public string UserId { get; set; }
        public string IPAddress { get; set; }
        public DateTime PromotionDateTime { get; set; }
        public int institute_id { get; set; }
    }
}
