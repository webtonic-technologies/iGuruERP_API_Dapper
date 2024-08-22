namespace Student_API.Models
{
    public class StudentMaster
    {
        public int student_id { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public int? gender_id { get; set; }
        public int? class_id { get; set; }
        public int? section_id { get; set; }
        public string Admission_Number { get; set; }
        public string Roll_Number { get; set; }
        public DateTime? Date_of_Joining { get; set; }
        public DateTime? Academic_Year { get; set; }
        public string Nationality_id { get; set; }
        public string Religion_id { get; set; }
        public DateTime? Date_of_Birth { get; set; }
        public int? Mother_Tongue_id { get; set; }
        public int? Caste_id { get; set; }
        public string First_Language { get; set; }
        public string Second_Language { get; set; }
        public string Third_Language { get; set; }
        //public string Medium { get; set; }
        public int? Blood_Group_id { get; set; }
        //public int? App_User_id { get; set; }
        public string Aadhar_Number { get; set; }
        public string PEN { get; set; }
        public string QR_code { get; set; }
    }
}
