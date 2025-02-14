namespace StudentManagement_API.DTOs.Requests
{
    public class StudentInformationImportRequest
    {
        public int StudentID { get; set; }
        public StudentDetails_IM StudentDetails { get; set; }
        public OtherInformation_IM OtherInformation { get; set; }
        public ParentsInfo_IM ParentsInfo { get; set; }
        public SiblingsDetails_IM SiblingsDetails { get; set; }
        public Documents_IM Documents { get; set; }
        public PreviousSchoolDetails_IM PreviousSchoolDetails { get; set; }
        public HealthInformation_IM HealthInformation { get; set; }
    }

    public class StudentDetails_IM
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string AdmissionNumber { get; set; }
        public string RollNumber { get; set; }
        public string DateOfJoining { get; set; }
        public string AcademicYear { get; set; }
        public int NationalityID { get; set; }
        public int ReligionID { get; set; }
        public string DateOfBirth { get; set; }
        public int MotherTongueID { get; set; }
        public int CasteID { get; set; }
        public int BloodGroupID { get; set; }
        public string AadharNo { get; set; }
        public string PEN { get; set; }
        public int StudentTypeID { get; set; }
        public int StudentHouseID { get; set; }
    }

    public class OtherInformation_IM
    {
        public string RegistrationDate { get; set; }
        public string RegistrationNo { get; set; }
        public string AdmissionDate { get; set; }
        public string SamagraID { get; set; }
        public string PlaceofBirth { get; set; }
        public string EmailID { get; set; }
        public string LanguageKnown { get; set; }
        public string Comments { get; set; }
        public string IdentificationMark1 { get; set; }
        public string IdentificationMark2 { get; set; }
    }

    public class ParentsInfo_IM
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PrimaryContactNo { get; set; }
        public string BankAccountNo { get; set; }
        public string BankIFSCCode { get; set; }
        public string FamilyRationCardType { get; set; }
        public string FamilyRationCardNo { get; set; }
        public string DateOfBirth { get; set; }
        public string AadharNo { get; set; }
        public string PANCardNo { get; set; }
        public string Occupation { get; set; }
        public string Designation { get; set; }
        public string NameoftheEmployer { get; set; }
        public string OfficeNo { get; set; }
        public string EmailID { get; set; }
        public decimal AnnualIncome { get; set; }
        public string ResidentialAddress { get; set; }
    }

    public class SiblingsDetails_IM
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string AdmissionNo { get; set; }
        public string DateOfBirth { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string InstituteName { get; set; }
        public string AadharNo { get; set; }
    }

    public class Documents_IM
    {
        public string DocumentFile { get; set; } // Can be Base64 or file path
    }

    public class PreviousSchoolDetails_IM
    {
        public string InstituteName { get; set; }
        public string Board { get; set; }
        public string Medium { get; set; }
        public string InstituteAddress { get; set; }
        public string Course { get; set; }
        public string Class { get; set; }
        public string TCNumber { get; set; }
        public string TCDate { get; set; }
        public bool IsTCSubmitted { get; set; }
    }

    public class HealthInformation_IM
    {
        public string Allergies { get; set; }
        public string Medications { get; set; }
        public string ConsultingDoctorsName { get; set; }
        public string ConsultingDoctorPhoneNumber { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string GovermentHealthID { get; set; }
        public int Vision { get; set; }
        public string Hearing { get; set; }
        public string Speech { get; set; }
        public string BehavioralProblems { get; set; }
        public string Chest { get; set; }
        public string HistoryofanyAccident { get; set; }
        public string AnyPhysicalDeformiity { get; set; }
        public string HistoryofMajorIllness { get; set; }
        public string AnyOtherRemarksOrWeakness { get; set; }
    }
}
