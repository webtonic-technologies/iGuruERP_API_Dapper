namespace Communication_API.DTOs.Requests.DigitalDiary
{
    public class AddUpdateDiaryRequest
    {
        public int DiaryID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectID { get; set; }
        public int StudentID { get; set; }
        public string Remarks { get; set; }
    }
}
