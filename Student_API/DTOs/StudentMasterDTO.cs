namespace Student_API.DTOs
{
    public class StudentMasterDTO
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
        public StudentOtherInfoDTO studentOtherInfoDTO { get; set; }
        public List<StudentParentInfoDTO> studentParentInfos { get; set; }
        public StudentSiblings studentSiblings { get; set; }  
        public StudentPreviousSchool studentPreviousSchool { get; set; }    
        public StudentHealthInfo studentHealthInfo { get; set; }    
    }

    public class StudentOtherInfoDTO
    {
        public int Student_Other_Info_id { get; set; }
        public int student_id { get; set; }
        public int? StudentType_id { get; set; }
        public string email_id { get; set; }
        public string Hall_Ticket_Number { get; set; }
        public int? Exam_Board_id { get; set; }
        public string Identification_Mark_1 { get; set; }
        public string Identification_Mark_2 { get; set; }
        public DateTime? Admission_Date { get; set; }
        public int? Student_Group_id { get; set; }
        public DateTime? Register_Date { get; set; }
        public string Register_Number { get; set; }
        public string samagra_ID { get; set; }
        public string Place_of_Birth { get; set; }
        public string comments { get; set; }
        public string language_known { get; set; }
    }
    public class StudentParentInfoDTO
    {
        public int Student_Parent_Info_id { get; set; }
        public int Student_id { get; set; }
        public int Parent_Type_id { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public string Contact_Number { get; set; }
        public string Bank_Account_no { get; set; }
        public string Bank_IFSC_Code { get; set; }
        public string Family_Ration_Card_Type { get; set; }
        public string Family_Ration_Card_no { get; set; }
        public string Mobile_Number { get; set; }
        public DateTime Date_of_Birth { get; set; }
        public string Aadhar_no { get; set; }
        public string PAN_card_no { get; set; }
        public string Residential_Address { get; set; }
        public int Occupation_id { get; set; }
        public string Designation { get; set; }
        public string Name_of_the_Employer { get; set; }
        public string Office_no { get; set; }
        public string Email_id { get; set; }
        public decimal Annual_Income { get; set; }
        public string File_Name { get; set; }
    }

    public class StudentSiblings
    {
        public int Student_Siblings_id { get; set; }
        public int Student_id { get; set; }
        public string Name { get; set; }
        public string Last_Name { get; set; }
        public string Admission_Number { get; set; }
        public DateTime Date_of_Birth { get; set; }
        public int Class_id { get; set; }
        public int Selection_id { get; set; }
        public string Institute_Name { get; set; }
        public string Aadhar_no { get; set; }
    }
    public class StudentPreviousSchool
    {
        public int Student_Prev_School_id { get; set; }
        public int student_id { get; set; }
        public string Previous_School_Name { get; set; }
        public string Previous_Board { get; set; }
        public string Previous_Medium { get; set; }
        public string Previous_School_Address { get; set; }
        public string Previous_School_Group { get; set; }
        public string Previous_Class { get; set; }
        public string TC_number { get; set; }
        public DateTime TC_date { get; set; }
        public bool isTC_Submitted { get; set; }
    }
    public class StudentHealthInfo
    {
        public int Student_Health_Info_id { get; set; }
        public int Student_id { get; set; }
        public string Allergies { get; set; }
        public string Medications { get; set; }
        public string Doctor_Name { get; set; }
        public string Doctor_Phone_no { get; set; }
        public float height { get; set; }
        public float weight { get; set; }
        public string Government_ID { get; set; }
        public DateTime BCG { get; set; }
        public DateTime MMR_Measles { get; set; }
        public DateTime Polio { get; set; }
    }
}
