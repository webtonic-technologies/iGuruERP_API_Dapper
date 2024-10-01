namespace TimeTable_API.DTOs.Responses
{
    public class ClassWiseResponse
    {
        public List<ClassDetail> ClassList { get; set; }

        public ClassWiseResponse()
        {
            ClassList = new List<ClassDetail>();
        }
    }

    public class ClassDetail
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public List<SectionDetail> SectionList { get; set; }

        public ClassDetail()
        {
            SectionList = new List<SectionDetail>();
        }
    }

    public class SectionDetail
    {
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public int NumberOfSessions { get; set; }
        public int NumberOfSubjects { get; set; }
    }
}
