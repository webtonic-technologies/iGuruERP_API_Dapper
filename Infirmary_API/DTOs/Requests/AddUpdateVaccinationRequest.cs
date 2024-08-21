namespace Infirmary_API.DTOs.Requests
{
    public class AddUpdateVaccinationRequest
    {
        public int VaccinationID { get; set; }
        public string VaccinationName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int InstituteID { get; set; }
    }
}
