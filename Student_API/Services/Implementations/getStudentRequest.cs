namespace Student_API.Services.Implementations
{
    public class getStudentRequest
    {
        public int Institute_id { get; set; }
        public int class_id { get; set; } = 0;
        public int section_id { get; set; } = 0;
        public int Academic_year_id { get; set; } = 0;
        public int StudentType_id { get; set; } = 0;
        public bool isActive { get; set; } = true;
        public int exportFormat { get; set; }   
    }
}
