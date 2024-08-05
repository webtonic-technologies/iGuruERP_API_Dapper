namespace Infirmary_API.DTOs.Requests
{
    public class AddUpdateStudentVaccinationRequest
    {
        public int StudentVaccinationID { get; set; }
        public string AcademicYear { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int StudentID { get; set; }
        public int VaccinationID { get; set; }
        public DateTime DateOfVaccination { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
