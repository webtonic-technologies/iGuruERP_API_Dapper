namespace Student_API.DTOs
{
    public class StudentDetailsDTO
    {
        public int student_id { get; set; }
        //public string First_Name { get; set; }
        //public string Last_Name { get; set; }
        public string Student_Name {  get; set; }       
        public string class_course { get; set; }
        public string Section { get; set; }
        public string Admission_Number { get; set; }
        public string Roll_Number { get; set; }
        public string Date_of_Joining { get; set; }
        public string Date_of_Birth { get; set; }
        public string Religion_Type { get; set; }
        public string Gender_Type { get; set; }
        public string Father_Name { get; set; }
    }
}
