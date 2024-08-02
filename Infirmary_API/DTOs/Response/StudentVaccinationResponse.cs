namespace Infirmary_API.DTOs.Response
{
    public class StudentVaccinationResponse
    {
        public int StudentVaccinationID { get; set; }
        public string AcademicYear { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; } // Additional field for class name
        public int SectionID { get; set; }
        public string SectionName { get; set; } // Additional field for section name
        public int StudentID { get; set; }
        public string StudentName { get; set; } // Additional field to include student name
        public int VaccinationID { get; set; }
        public DateTime DateOfVaccination { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
