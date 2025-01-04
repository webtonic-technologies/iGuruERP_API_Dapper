namespace Infirmary_API.DTOs.Responses
{
    public class GetStockSummaryReportResponse
    {
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Company { get; set; }
        public string BatchCode { get; set; }
        public int OpeningStock { get; set; }
        public int InQuantity { get; set; }
        public int OutQuantity { get; set; }
        public int ClosingStock { get; set; }
    }
}
