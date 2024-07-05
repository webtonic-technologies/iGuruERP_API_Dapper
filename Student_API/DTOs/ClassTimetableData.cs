namespace Student_API.DTOs
{
    public class ClassTimetableData
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int Sessions { get; set; }
        public int Subjects { get; set; }
    }
    public class ClassTimetableResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public List<ClassData> ClassData { get; set; }
    }

    public class ClassData
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int Sessions { get; set; }
        public int Subjects { get; set; }
    }
}
