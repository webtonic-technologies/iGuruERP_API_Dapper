﻿using System.ComponentModel.DataAnnotations;

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

    public class StudentAllInformationDTO
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
    }
}
