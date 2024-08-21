namespace Infirmary_API.DTOs.Response
{
    public class VaccinationFetchResponse
    {
        public int VaccinationID { get; set; }
        public string VaccinationName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
