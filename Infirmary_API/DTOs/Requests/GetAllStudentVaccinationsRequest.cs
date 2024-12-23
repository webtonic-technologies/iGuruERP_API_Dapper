namespace Infirmary_API.DTOs.Requests
{
    public class GetAllStudentVaccinationsRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; } // Added ClassID for filtering
        public int SectionID { get; set; } // Added SectionID for filtering
        public int VaccinationID { get; set; } // Added VaccinationID for filtering
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
