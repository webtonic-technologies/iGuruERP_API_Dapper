﻿using Student_API.Helper;
using System.ComponentModel.DataAnnotations;

namespace Student_API.DTOs.RequestDTO
{
    public class StudentMasters
    {
        public int student_id { get; set; }
        [Required]
        [MaxLength(50)]
        public string First_Name { get; set; }

        [MaxLength(50)]
        public string Middle_Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string Last_Name { get; set; }
        public int? gender_id { get; set; }
        public int? class_id { get; set; }
        public int? section_id { get; set; }
        [MaxLength(20)]
        public string Admission_Number { get; set; }
        [MaxLength(20)]
        public string Roll_Number { get; set; }
        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string Date_of_Joining { get; set; }
        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string Academic_Year { get; set; }
        [Required]
        public int? Nationality_id { get; set; }
        [Required]
        public int? Religion_id { get; set; }
        [Required]
        [ValidDateString("dd-MM-yyyy hh:mm tt")]
        public string Date_of_Birth { get; set; }
        [Required]
        public int? Mother_Tongue_id { get; set; }

        public int? Caste_id { get; set; }
        public string First_Language { get; set; }
        public string Second_Language { get; set; }
        public string Third_Language { get; set; }
        //public string Medium { get; set; }
        public int? Blood_Group_id { get; set; }
        //public int? App_User_id { get; set; }
        [MaxLength(20)]
        public string Aadhar_Number { get; set; }
        public string PEN { get; set; }
        public string QR_code { get; set; }
        public bool IsPhysicallyChallenged { get; set; }
        public bool IsSports { get; set; }
        public bool IsAided { get; set; }
        public bool IsNCC { get; set; }
        public bool IsNSS { get; set; }
        public bool IsScout { get; set; }
        public string? File_Name { get; set; }
        public int Institute_id { get; set; }
        public int? StudentType_id { get; set; }

    }
    public class StudentOtherInfos
    {
        public int Student_Other_Info_id { get; set; }
        public int student_id { get; set; }
        
        [MaxLength(50)]
        public string email_id { get; set; }
        [MaxLength(30)]
        public string Identification_Mark_1 { get; set; }
        [MaxLength(30)]
        public string Identification_Mark_2 { get; set; }
        [ValidDateString("dd-MM-yyyy")]
        public string Admission_Date { get; set; }
      
        [ValidDateString("dd-MM-yyyy")]
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

    }
    public class StudentParentInfo
    {
        public int Student_Parent_Info_id { get; set; }
        public int Student_id { get; set; }
        public int Parent_Type_id { get; set; }
        [MaxLength(30)]
        public string First_Name { get; set; }
        [MaxLength(30)]
        public string Middle_Name { get; set; }
        [MaxLength(30)]
        public string Last_Name { get; set; }
        [MaxLength(20)]
        public string Mobile_Number { get; set; }
        [MaxLength(30)]
        public string Bank_Account_no { get; set; }
        [MaxLength(30)]
        public string Bank_IFSC_Code { get; set; }
        [MaxLength(30)]
        public string Family_Ration_Card_Type { get; set; }
        [MaxLength(30)]
        public string Family_Ration_Card_no { get; set; }
        [ValidDateString("dd-MM-yyyy")]
        public string Date_of_Birth { get; set; }
        [MaxLength(20)]
        public string Aadhar_no { get; set; }
        [MaxLength(30)]
        public string PAN_card_no { get; set; }
        [MaxLength(30)]
        public string Residential_Address { get; set; }
        public string Occupation { get; set; }
        [MaxLength(30)]
        public string Designation { get; set; }
        [MaxLength(30)]
        public string Name_of_the_Employer { get; set; }
        [MaxLength(20)]
        public string Office_no { get; set; }
        [MaxLength(30)]
        public string Email_id { get; set; }

        public decimal Annual_Income { get; set; }
        public string File_Name { get; set; }

        public StudentParentOfficeInfos studentParentOfficeInfo { get; set; }
    }

    public class StudentParentOfficeInfos
    {
        public int Student_Parent_Office_Info_id { get; set; }
        public int Student_id { get; set; }
        public int Parents_Type_id { get; set; }
        [MaxLength(30)]
        public string Office_Building_no { get; set; }
        [MaxLength(30)]
        public string Street { get; set; }
        [MaxLength(30)]
        public string Area { get; set; }
      
        //public int City_id { get; set; }
      
        //public int State_id { get; set; }
        public string City {  get; set; }   
        public string State {  get; set; }   

        [MaxLength(30)]
        public string Pincode { get; set; }
    }


    public class StudentSibling
    {
        public int Student_Siblings_id { get; set; }
        public int Student_id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string Last_Name { get; set; }
        [MaxLength(30)]
        public string Middle_Name { get; set; }
        [MaxLength(30)]
        public string Admission_Number { get; set; }
        [ValidDateString("dd-MM-yyyy")]
        public string Date_of_Birth { get; set; }
        public string Class { get; set; }
        public string section  { get; set; }
        [MaxLength(30)]
        public string Institute_Name { get; set; }
        [MaxLength(20)]
        public string Aadhar_no { get; set; }
    }
    public class StudentPreviousSchools
    {
        public int Student_Prev_School_id { get; set; }
        public int student_id { get; set; }
        [MaxLength(200)]
        public string Previous_School_Name { get; set; }
        [MaxLength(50)]
        public string Previous_Board { get; set; }
        [MaxLength(20)]
        public string Previous_Medium { get; set; }
        [MaxLength(200)]
        public string Previous_School_Address { get; set; }
        [MaxLength(50)]
        public string previous_School_Course { get; set; }
        [MaxLength(50)]
        public string Previous_Class { get; set; }
        [MaxLength(50)]
        public string TC_number { get; set; }
        [ValidDateString("dd-MM-yyyy")]
        public string TC_date { get; set; }
        public bool isTC_Submitted { get; set; }
    }
    public class StudentHealthInfos
    {
        public int Student_Health_Info_id { get; set; }
        public int Student_id { get; set; }
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
        [MaxLength(50)]
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
       
    }
}
