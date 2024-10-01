namespace TimeTable_API.DTOs.Responses
{
    public class ClassSessionResponse
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
}
