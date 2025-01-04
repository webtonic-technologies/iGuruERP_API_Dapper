namespace Infirmary_API.DTOs.Responses
{
    public class GetStockInfoResponse
    {
        public string InfirmaryName { get; set; }
        public string CompanyName { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public string Diagnosis { get; set; }
    }
}
