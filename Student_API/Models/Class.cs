namespace Student_API.Models
{
    public class Class
    {
        public int class_id { get; set; }
        public string class_name { get; set; }
    }

    public class ClassWithSections
    {
        public int class_id { get; set; }
        public string class_name { get; set; }
        public List<Sections> sections { get; set; }
    }
}
