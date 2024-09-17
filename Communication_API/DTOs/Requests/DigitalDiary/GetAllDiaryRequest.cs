namespace Communication_API.DTOs.Requests.DigitalDiary
{
    public class GetAllDiaryRequest
    {
        public int InstituteID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
