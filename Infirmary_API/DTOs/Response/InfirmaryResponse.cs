namespace Infirmary_API.DTOs.Response
{
    public class InfirmaryResponse
    {
        public string InfirmaryID { get; set; }
        public string InfirmaryName { get; set; }
        public string InfirmaryIncharge { get; set; }
        public int NoOfBeds { get; set; }
        public string Description { get; set; }
    }
}
