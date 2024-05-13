namespace Student_API.DTOs
{
    public class StudentInformationDTO
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
        public string Medium { get; set; }
        public int? Blood_Group_id { get; set; }
        public int? App_User_id { get; set; }
        public string Aadhar_Number { get; set; }
        public string NEP { get; set; }
        public string QR_code { get; set; }
        public bool IsPhysicallyChallenged { get; set; }
        public bool IsSports { get; set; }
        public bool IsAided { get; set; }
        public bool IsNCC { get; set; }
        public bool IsNSS { get; set; }
        public bool IsScout { get; set; }
        public string File_Name { get; set; }
        public string Base64File { get; set; }
        public StudentOtherInfoDTO studentOtherInfoDTO { get; set; }
        public List<StudentParentInfoDTO> studentParentInfos { get; set; }
        public StudentSiblings studentSiblings { get; set; }
        public StudentPreviousSchool studentPreviousSchool { get; set; }
        public StudentHealthInfo studentHealthInfo { get; set; }
        public List<StudentDocumentListDTO> studentDocumentListDTOs{ get; set; }
    }
}
