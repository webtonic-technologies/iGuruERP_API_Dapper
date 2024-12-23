namespace Infirmary_API.DTOs.Responses
{
    public class GetInfirmaryExportResponse
    {
        public string InfirmaryName { get; set; }
        public string InfirmaryIncharge { get; set; }
        public int NoOfBeds { get; set; }
        public string Description { get; set; }
    }
}
