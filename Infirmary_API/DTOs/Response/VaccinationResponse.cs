namespace Infirmary_API.DTOs.Response
{
    public class VaccinationResponse
    {
        public int VaccinationID { get; set; }
        public string VaccinationName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }
}
