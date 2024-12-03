using System.ComponentModel.DataAnnotations;

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
        public string AcademicYearCode { get; set; }
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

    public class StudentAllInformationDTO
    {
        public int student_id { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public int? gender_id { get; set; }
        public string Gender_Type { get; set; }
        public int? class_id { get; set; }
        public string class_name { get; set; }
        public int? section_id { get; set; }
        public string section_name { get; set; }
        public string Admission_Number { get; set; }
        public string Roll_Number { get; set; }
        public string Date_of_Joining { get; set; }
        public string AcademicYearCode { get; set; }
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
        public int Student_Other_Info_id { get; set; }

        //public int? StudentType_id { get; set; }
        //public string? Student_Type_Name { get; set; }
        [MaxLength(50)]
        public string email_id { get; set; }

        [MaxLength(30)]
        public string Identification_Mark_1 { get; set; }
        [MaxLength(30)]
        public string Identification_Mark_2 { get; set; }
        public string Admission_Date { get; set; }
        //public int? Student_House_id { get; set; }
        //public string? HouseName { get; set; }
        public string Register_Date { get; set; }
        [MaxLength(30)]
        public string Register_Number { get; set; }
        [MaxLength(30)]
        public string samagra_ID { get; set; }
        [MaxLength(30)]
        public string Place_of_Birth { get; set; }
        [MaxLength(30)]
        public string comments { get; set; }
        [MaxLength(30)]
        public string language_known { get; set; }
        public int Student_Prev_School_id { get; set; }

        public string Previous_School_Name { get; set; }

        public string Previous_Board { get; set; }

        public string Previous_Medium { get; set; }

        public string Previous_School_Address { get; set; }

        public string previous_School_Course { get; set; }

        public string Previous_Class { get; set; }

        public string TC_number { get; set; }
        public string TC_date { get; set; }
        public bool isTC_Submitted { get; set; }
        public int Student_Health_Info_id { get; set; }

        [MaxLength(50)]
        public string Allergies { get; set; }

        [MaxLength(50)]
        public string Medications { get; set; }
        [MaxLength(50)]
        public string Doctor_Name { get; set; }
        [MaxLength(50)]
        public string Doctor_Phone_no { get; set; }

        public float height { get; set; }

        public float weight { get; set; }
        [MaxLength(50)]
        public string Government_ID { get; set; }

        public string Chest { get; set; }

        [MaxLength(50)]
        public string Physical_Deformity { get; set; }
        [MaxLength(50)]
        public string History_Majorillness { get; set; }
        [MaxLength(50)]
        public string History_Accident { get; set; }

        public decimal? Vision { get; set; }
        [MaxLength(50)]
        public string Hearing { get; set; }
        [MaxLength(50)]
        public string Speech { get; set; }
        [MaxLength(50)]
        public string Behavioral_Problem { get; set; }
        [MaxLength(50)]
        public string Remarks_Weakness { get; set; }
        [MaxLength(50)]
        public string Student_Name { get; set; }
        [Range(0, 99)]
        public int Student_Age { get; set; }

        // Father's Details
        public string Father_First_Name { get; set; }
        public string Father_Middle_Name { get; set; }
        public string Father_Last_Name { get; set; }
        public string Father_Bank_Account_no { get; set; }
        public string Father_Bank_IFSC_Code { get; set; }
        public string Father_Family_Ration_Card_Type { get; set; }
        public string Father_Family_Ration_Card_no { get; set; }
        public string Father_Mobile_Number { get; set; }
        public string Father_Date_of_Birth { get; set; }
        public string Father_Aadhar_no { get; set; }
        public string Father_PAN_card_no { get; set; }
        public string Father_Residential_Address { get; set; }
        public string Father_Occupation_Type { get; set; }
        public string Father_Designation { get; set; }
        public string Father_Name_of_the_Employer { get; set; }
        public string Father_Office_no { get; set; }
        public string Father_Email_id { get; set; }
        public decimal Father_Annual_Income { get; set; }
        public string Father_File_Name { get; set; }

        // Mother's Details
        public string Mother_First_Name { get; set; }
        public string Mother_Middle_Name { get; set; }
        public string Mother_Last_Name { get; set; }
        public string Mother_Bank_Account_no { get; set; }
        public string Mother_Bank_IFSC_Code { get; set; }
        public string Mother_Family_Ration_Card_Type { get; set; }
        public string Mother_Family_Ration_Card_no { get; set; }
        public string Mother_Mobile_Number { get; set; }
        public string Mother_Date_of_Birth { get; set; }
        public string Mother_Aadhar_no { get; set; }
        public string Mother_PAN_card_no { get; set; }
        public string Mother_Residential_Address { get; set; }
        public string Mother_Occupation_Type { get; set; }
        public string Mother_Designation { get; set; }
        public string Mother_Name_of_the_Employer { get; set; }
        public string Mother_Office_no { get; set; }
        public string Mother_Email_id { get; set; }
        public decimal Mother_Annual_Income { get; set; }
        public string Mother_File_Name { get; set; }

        // Guardian's Details
        public string Guardian_First_Name { get; set; }
        public string Guardian_Middle_Name { get; set; }
        public string Guardian_Last_Name { get; set; }
        public string Guardian_Bank_Account_no { get; set; }
        public string Guardian_Bank_IFSC_Code { get; set; }
        public string Guardian_Family_Ration_Card_Type { get; set; }
        public string Guardian_Family_Ration_Card_no { get; set; }
        public string Guardian_Mobile_Number { get; set; }
        public string Guardian_Date_of_Birth { get; set; }
        public string Guardian_Aadhar_no { get; set; }
        public string Guardian_PAN_card_no { get; set; }
        public string Guardian_Residential_Address { get; set; }
        public string Guardian_Occupation_Type { get; set; }
        public string Guardian_Designation { get; set; }
        public string Guardian_Name_of_the_Employer { get; set; }
        public string Guardian_Office_no { get; set; }
        public string Guardian_Email_id { get; set; }
        public decimal Guardian_Annual_Income { get; set; }
        public string Guardian_File_Name { get; set; }

        // Father's Office Info
        public string Father_Office_Building_no { get; set; }
        public string Father_Street { get; set; }
        public string Father_Area { get; set; }
        public string Father_Pincode { get; set; }
        public string Father_City { get; set; }
        public string Father_State { get; set; }

        // Mother's Office Info
        public string Mother_Office_Building_no { get; set; }
        public string Mother_Street { get; set; }
        public string Mother_Area { get; set; }
        public string Mother_Pincode { get; set; }
        public string Mother_City { get; set; }
        public string Mother_State { get; set; }

        // Guardian's Office Info
        public string Guardian_Office_Building_no { get; set; }
        public string Guardian_Street { get; set; }
        public string Guardian_Area { get; set; }
        public string Guardian_Pincode { get; set; }
        public string Guardian_City { get; set; }
        public string Guardian_State { get; set; }

        // Sibling Details
        public string SiblingDetails { get; set; }
    }
}
