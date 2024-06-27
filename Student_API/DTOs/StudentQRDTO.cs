namespace Student_API.DTOs
{
    public class StudentQRDTO
    {
        public int student_id { get; set; }
        //public string First_Name { get; set; }
        //public string Last_Name { get; set; }
        public string Student_Name { get; set; }
        public int? class_id { get; set; }
        public string class_course { get; set; }
        public int? section_id { get; set; }
        public string Section { get; set; }
        public string Admission_Number { get; set; }
        public string Roll_Number { get; set; }
        public string QR_code { get; set; }
    }
}
