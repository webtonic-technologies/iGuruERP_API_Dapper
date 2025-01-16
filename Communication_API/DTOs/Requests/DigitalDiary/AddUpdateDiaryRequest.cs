namespace Communication_API.DTOs.Requests.DigitalDiary
{
    public class AddUpdateDiaryRequest
    {
        public int DiaryID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectID { get; set; }
        public List<int> StudentIDs { get; set; } // Changed to a list of StudentIDs
        public string Remarks { get; set; }
        public int InstituteID { get; set; }
        public int CreatedBy { get; set; }
    }
}
