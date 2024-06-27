namespace Student_API.DTOs
{
    public class StudentPromotionDTO
    {
        public int student_id { get; set; }
        //public string first_name { get; set; }
        //public string last_name { get; set; }
        public string Student_Name {  get; set; }   
        public int class_id { get; set; }
        //public string ClassName { get; set; }
        public int Section_Id { get; set; }
        //public string SectionName { get; set; }
        public string Class_Section { get; set; }
    }
}
