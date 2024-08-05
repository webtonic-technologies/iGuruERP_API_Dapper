namespace Infirmary_API.Models
{
    public class Vaccination
    {
        public int VaccinationID { get; set; }
        public string VaccinationName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }
}
