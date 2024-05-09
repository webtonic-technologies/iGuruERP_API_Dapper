namespace Institute_API.Models
{
    public class SemesterInfo
    {
        public int SemesterInfoId { get; set; }
        public int Institute_id { get; set; }
        public bool? IsSemester { get; set; }
        public DateTime SemesterStartDate { get; set; }
        public DateTime SemesterEndDate { get; set; }
    }
}
