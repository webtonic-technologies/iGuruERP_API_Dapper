namespace StudentManagement_API.DTOs.Requests
{
    public class GetStudentsRequest
    {
        public int InstituteID { get; set; }
        public string AcademicYearCode { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int TemplateID { get; set; }
    }
}
