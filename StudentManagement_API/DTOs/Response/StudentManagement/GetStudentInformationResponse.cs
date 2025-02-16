using System;

namespace StudentManagement_API.DTOs.Responses
{
    public class GetStudentInformationResponse
    {
        // Basic Student Info
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNumber { get; set; }
        public string DateOfJoining { get; set; }
        public string Nationality { get; set; }
        public string Religion { get; set; }
        public string DateOfBirth { get; set; }
        public string MotherTongue { get; set; }
        public string Caste { get; set; }
        public string BloodGroup { get; set; }
        public string AadharNo { get; set; }
        public string PEN { get; set; }
        public string QRCode { get; set; }
        public string PhysicallyChallenged { get; set; }
        public string Sports { get; set; }
        public string Aided { get; set; }
        public string NCC { get; set; }
        public string NSS { get; set; }
        public string Scout { get; set; }
        public string FileName { get; set; }
        public string IsActive { get; set; }
        public string InstituteID { get; set; }
        public string InstituteHouseID { get; set; }
        public string StudentType { get; set; }
        public string AcademicYearCode { get; set; }

        // Student Other Info
        public string StudentOtherInfoID { get; set; }
        public string EmailID { get; set; }
        public string HallTicketNumber { get; set; }
        public string IdentificationMark1 { get; set; }
        public string IdentificationMark2 { get; set; }
        public string AdmissionDate { get; set; }
        public string RegisterDate { get; set; }
        public string RegisterNumber { get; set; }
        public string SamagraID { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Comments { get; set; }
        public string LanguageKnown { get; set; }
        public string MobileNumber { get; set; }

        // Father Info
        public string FatherFirstName { get; set; }
        public string FatherMiddleName { get; set; }
        public string FatherLastName { get; set; }
        public string FatherMobileNumber { get; set; }
        public string FatherBankAccountNo { get; set; }
        public string FatherBankIFSCCode { get; set; }
        public string FatherRationCardType { get; set; }
        public string FatherRationCardNo { get; set; }
        public string FatherDateOfBirth { get; set; }
        public string FatherAadharNo { get; set; }
        public string FatherPANCardNo { get; set; }
        public string FatherResidentialAddress { get; set; }
        public string FatherDesignation { get; set; }
        public string FatherEmployerName { get; set; }
        public string FatherOfficeNo { get; set; }
        public string FatherEmailID { get; set; }
        public string FatherAnnualIncome { get; set; }
        public string FatherOccupation { get; set; }

        // Mother Info
        public string MotherFirstName { get; set; }
        public string MotherMiddleName { get; set; }
        public string MotherLastName { get; set; }
        public string MotherMobileNumber { get; set; }
        public string MotherBankAccountNo { get; set; }
        public string MotherBankIFSCCode { get; set; }
        public string MotherRationCardType { get; set; }
        public string MotherRationCardNo { get; set; }
        public string MotherDateOfBirth { get; set; }
        public string MotherAadharNo { get; set; }
        public string MotherPANCardNo { get; set; }
        public string MotherResidentialAddress { get; set; }
        public string MotherDesignation { get; set; }
        public string MotherEmployerName { get; set; }
        public string MotherOfficeNo { get; set; }
        public string MotherEmailID { get; set; }
        public string MotherAnnualIncome { get; set; }
        public string MotherOccupation { get; set; }

        // Guardian Info
        public string GuardianFirstName { get; set; }
        public string GuardianMiddleName { get; set; }
        public string GuardianLastName { get; set; }
        public string GuardianMobileNumber { get; set; }
        public string GuardianBankAccountNo { get; set; }
        public string GuardianBankIFSCCode { get; set; }
        public string GuardianRationCardType { get; set; }
        public string GuardianRationCardNo { get; set; }
        public string GuardianDateOfBirth { get; set; }
        public string GuardianAadharNo { get; set; }
        public string GuardianPANCardNo { get; set; }
        public string GuardianResidentialAddress { get; set; }
        public string GuardianDesignation { get; set; }
        public string GuardianEmployerName { get; set; }
        public string GuardianOfficeNo { get; set; }
        public string GuardianEmailID { get; set; }
        public string GuardianAnnualIncome { get; set; }
        public string GuardianOccupation { get; set; }

        // Sibling Info
        public string StudentSiblingsID { get; set; }
        public string SiblingStudentID { get; set; }
        public string SiblingName { get; set; }
        public string SiblingLastName { get; set; }
        public string SiblingAdmissionNo { get; set; }
        public string SiblingDateOfBirth { get; set; }
        public string SiblingInstituteName { get; set; }
        public string SiblingAadharNo { get; set; }
        public string SiblingClass { get; set; }
        public string SiblingSection { get; set; }
        public string SiblingMiddleName { get; set; }

        // Document Info
        public string StudentDocumentsID { get; set; }
        public string DocumentStudentID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentFileName { get; set; }
        public string DocumentFilePath { get; set; }
        public string IsDeletedDocument { get; set; }

        // Previous School Info
        public string StudentPreviousSchoolID { get; set; }
        public string PrevSchoolStudentID { get; set; }
        public string PreviousSchoolName { get; set; }
        public string PreviousBoard { get; set; }
        public string PreviousMedium { get; set; }
        public string PreviousSchoolAddress { get; set; }
        public string PreviousSchoolCourse { get; set; }
        public string PreviousClass { get; set; }
        public string TCNumber { get; set; }
        public string TCDate { get; set; }
        public string TCSubmitted { get; set; }

        // Parent Office Info
        public string StudentParentOfficeInfoID { get; set; }
        public string ParentOfficeStudentID { get; set; }
        public string ParentTypeIDOffice { get; set; }
        public string OfficeBuildingNo { get; set; }
        public string Street { get; set; }
        public string Area { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        // Health Info
        public string StudentHealthInfoID { get; set; }
        public string HealthStudentID { get; set; }
        public string Allergies { get; set; }
        public string Medications { get; set; }
        public string DoctorName { get; set; }
        public string DoctorPhoneNo { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string GovernmentHealthID { get; set; }
        public string Chest { get; set; }
        public string PhysicalDeformity { get; set; }
        public string HistoryOfMajorIllness { get; set; }
        public string HistoryOfAccident { get; set; }
        public string Vision { get; set; }
        public string Hearing { get; set; }
        public string Speech { get; set; }
        public string BehavioralProblem { get; set; }
        public string RemarksWeakness { get; set; }
        public string StudentNameHealth { get; set; }
        public string StudentAge { get; set; }
        public string AdmissionStatus { get; set; }
    }
}



//using System.Xml.Linq;

//namespace StudentManagement_API.DTOs.Responses
//{
//    public class GetStudentInformationResponse
//    {
//        public int StudentID { get; set; }
//        public string AdmissionNumber { get; set; }
//        public string StudentName { get; set; }
//        public string Class { get; set; }
//        public string Section { get; set; }
//        public string RollNumber { get; set; }
//        public string Gender { get; set; }
//        public string DateOfBirth { get; set; }
//        public string Religion { get; set; }
//        public string DateOfJoining { get; set; }
//        public string FatherName { get; set; }
//        public string SiblingFirstName { get; set; }
//        public string SiblingLastName { get; set; }
//        public string SiblingAdmissionNumber { get; set; }
//        public string SiblingClass { get; set; }
//        public string SiblingSection { get; set; }
//        public string SiblingInstituteName { get; set; }
//        public string SiblingAadharNumber { get; set; }
//    }
//}
