namespace Infirmary_API.DTOs.Responses
{
    public class GetStockSummaryReportExportResponse
    {
        public string ItemType { get; set; }   // The type of item (e.g., Medicals, Surgical)
        public string ItemName { get; set; }   // Name of the item (e.g., Paracetamol)
        public string Company { get; set; }    // Company name (e.g., ABC Pharmaceuticals)
        public string BatchCode { get; set; }  // Batch code (e.g., ABC12345)
        public int OpeningStock { get; set; }  // Opening stock quantity
        public int InQuantity { get; set; }    // Quantity added to stock
        public int OutQuantity { get; set; }   // Quantity removed from stock
        public int ClosingStock { get; set; }  // Closing stock quantity (OpeningStock - OutQuantity + InQuantity)
    }
}
