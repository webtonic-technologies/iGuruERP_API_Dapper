using Student_API.Helper;
using System.ComponentModel.DataAnnotations;

namespace Student_API.DTOs.RequestDTO
{
    public class StudentDTO
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
        [ValidDateString("dd-MM-yyyy")]
        public string Date_of_Joining { get; set; }
        public int Academic_year_id { get; set; }
        [Required]
        public int? Nationality_id { get; set; }
        [Required]
        public int? Religion_id { get; set; }
        [Required]
        [ValidDateString("dd-MM-yyyy")]
        public string Date_of_Birth { get; set; }
        [Required]
        public int? Mother_Tongue_id { get; set; }

        public int? Caste_id { get; set; }
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
        public int Student_House_id { get; set; }
        public int StudentType_id { get; set; }
        public StudentOtherInfos StudentOtherInfos { get; set; }
        public List<StudentParentInfo> studentParentInfos { get; set; } 
        public List<StudentSibling> studentSiblings { get; set; } 
        public StudentPreviousSchools studentPreviousSchools { get; set; } 
        public StudentHealthInfos studentHealthInfos { get; set; } 
        public StudentDocumentsDTO studentDocuments { get; set; }    
    }
}
