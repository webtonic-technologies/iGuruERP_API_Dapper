namespace Student_API.DTOs
{
    public class StudentInformationDTO
    {
        public int student_id { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public int? gender_id { get; set; }
        public string Gender_Type { get; set; }
        public int? class_id { get; set; }
        public string class_course { get; set; }
        public int? section_id { get; set; }
        public string Section { get; set; }
        public string Admission_Number { get; set; }
        public string Roll_Number { get; set; }
        public string Date_of_Joining { get; set; }
        public int Academic_year_id { get; set; }
        public string YearName { get; set; }
        public int? Nationality_id { get; set; }
        public string Nationality_Type { get; set; }
        public string Religion_id { get; set; }
        public string Religion_Type { get; set; }
        public string Date_of_Birth { get; set; }
        public int? Mother_Tongue_id { get; set; }
        public string Mother_Tongue_Name { get; set; }
        public int? Caste_id { get; set; }
        public string caste_type { get; set; }
        //public string Medium { get; set; }
        public int? Blood_Group_id { get; set; }
        public string Blood_Group_Type { get; set; }
        //public int? App_User_id { get; set; }
        public string Aadhar_Number { get; set; }
        public string PEN { get; set; }
        public string QR_code { get; set; }
        public bool IsPhysicallyChallenged { get; set; }
        public bool IsSports { get; set; }
        public bool IsAided { get; set; }
        public bool IsNCC { get; set; }
        public bool IsNSS { get; set; }
        public bool IsScout { get; set; }
        public string File_Name { get; set; }
        public string Father_Name { get; set; }
        public int Institute_id { get; set; }
        public string? Institute_name { get; set; }
        public int? Student_House_id { get; set; }
        public string? Student_House_Name { get; set; }
        public int? StudentType_id { get; set; }
        public string? Student_Type_Name { get; set; }
        public StudentOtherInfoDTO studentOtherInfoDTO { get; set; }
        public List<StudentParentInfoDTO> studentParentInfos { get; set; }
        public List<StudentSiblings> studentSiblings { get; set; }
        public StudentPreviousSchool studentPreviousSchool { get; set; }
        public StudentHealthInfo studentHealthInfo { get; set; }
        public List<StudentDocumentListDTO> studentDocumentListDTOs { get; set; }
    }
}
